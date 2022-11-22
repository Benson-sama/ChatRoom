//---------------------------------------------------------------------
// <copyright file="ServerApplication.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ServerApplication class.</summary>
//---------------------------------------------------------------------
namespace ChatRoom.Model.Applications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using ChatRoom.Model.ConsoleModel.Watcher;
    using ChatRoom.Model.EnhancedTCPC;
    using ChatRoom.Model.Messages;
    using ChatRoom.Model.Serialiser;

    /// <summary>
    /// Represents the <see cref="ServerApplication"/> class.
    /// </summary>
    public class ServerApplication
    {
        /// <summary>
        /// The <see cref="KeyboardWatcher"/> of this <see cref="ServerApplication"/>.
        /// </summary>
        private readonly KeyboardWatcher keyboardWatcher;

        /// <summary>
        /// The participating users of this <see cref="ServerApplication"/>.
        /// </summary>
        private readonly List<User> users;

        /// <summary>
        /// The accepted connections as <see cref="EnhancedTcpClient"/> of this <see cref="ServerApplication"/>.
        /// </summary>
        private readonly List<EnhancedTcpClient> clients;

        /// <summary>
        /// The <see cref="MessageParser"/> instances of this <see cref="ServerApplication"/>.
        /// </summary>
        private readonly List<MessageParser> messageParser;

        /// <summary>
        /// The <see cref="TcpListener"/> of this <see cref="ServerApplication"/>.
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// The value indicating whether this <see cref="ServerApplication"/> is active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Cache for keeping track of the next user id.
        /// </summary>
        private int nextUserId;

        /// <summary>
        /// Initialises a new instance of the <see cref="ServerApplication"/> class.
        /// </summary>
        public ServerApplication()
        {
            this.keyboardWatcher = new KeyboardWatcher();
            this.users = new List<User>();
            this.clients = new List<EnhancedTcpClient>();
            this.messageParser = new List<MessageParser>();
            this.nextUserId = 1;
            this.keyboardWatcher.OnKeyPressed += this.KeyboardWatcher_OnKeyPressed;
        }

        /// <summary>
        /// The event that gets fired when an action gets logged.
        /// </summary>
        public event EventHandler<ActionLoggedEventArgs> ActionLogged;

        /// <summary>
        /// Starts this <see cref="ServerApplication"/>.
        /// </summary>
        public void Run()
        {
            Console.Clear();
            Console.CursorVisible = true;

            int serverPort = this.GetPort("Please enter the port number on which the server listens: ");
            this.listener = new TcpListener(IPAddress.Any, serverPort);
            this.keyboardWatcher.Start();
            this.listener.Start();
            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, "Listening for incoming connection requests..."));
            this.isActive = true;
            while (this.isActive)
            {
                if (!this.listener.Pending())
                {
                    Thread.Sleep(250);
                    continue;
                }

                try
                {
                    TcpClient client = this.listener.AcceptTcpClient();
                    this.ExecuteAddNewClientCommand(client);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Sets up the environment for the recently joined <see cref="TcpClient"/> and initiates
        /// distribution of the necessary messages to the new client and to all other clients.
        /// </summary>
        /// <param name="client">The recently joined <see cref="TcpClient"/>.</param>
        private void ExecuteAddNewClientCommand(TcpClient client)
        {
            User newUser = new User(this.nextUserId, this.nextUserId.ToString(), "gray", client.Client.RemoteEndPoint.ToString());
            this.nextUserId++;
            EnhancedTcpClient newClient = new EnhancedTcpClient(client);
            MessageParser messageParser = new MessageParser(newClient, 8);
            this.users.Add(newUser);
            this.clients.Add(newClient);
            this.messageParser.Add(messageParser);
            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, $"New client connected, I'll give him ID {newUser.Id}."));

            newClient.DataReceived += messageParser.Collect;
            messageParser.MessageParsed += this.MessageParser_MessageParsed;
            newClient.StartDataMonitoring();
            newClient.StartAliveMonitoring();

            this.ExecuteSendInitialMessagesToClientCommand(newUser, messageParser);

            // Notify all other clients about the new user.
            var otherParser = this.messageParser.Where(x => x.EnhancedTcpClient != newClient).ToList();
            var userJoinedMessage = new UserUpdateMessage("joined", newUser);
            var serialisedUserJoinedMessage = UserUpdateMessageSerialiser.Serialise(userJoinedMessage);
            otherParser.ForEach(x => x.ParseAndSend(serialisedUserJoinedMessage));
            var serverMessage = new ServerMessage(DateTime.Now, "Server", "Green", $"\"{newUser.Nickname}\" entered the chat.");
            var serialisedServerMessage = ServerMessageSerialiser.Serialise(serverMessage);
            otherParser.ForEach(x => x.ParseAndSend(serialisedServerMessage));
        }

        /// <summary>
        /// Sends the local user, the current list of users and the welcome message to the new client.
        /// </summary>
        /// <param name="newUser">The recently joined <see cref="User"/>.</param>
        /// <param name="messageParser">The <see cref="MessageParser"/> of the corresponding client.</param>
        private void ExecuteSendInitialMessagesToClientCommand(User newUser, MessageParser messageParser)
        {
            // Send the new user to the new client.
            var userUpdate = new UserUpdateMessage("local", newUser);
            var newUserData = UserUpdateMessageSerialiser.Serialise(userUpdate);
            messageParser.ParseAndSend(newUserData);

            // Send the current list of users.
            var userListData = UserListSerialiser.Serialise(this.users.Where(x => x != newUser).ToList());
            messageParser.ParseAndSend(userListData);
            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, $"Sent list of current users to \"{newUser.Nickname}\"."));

            // Send the welcome message.
            var welcomeMessageData = ServerMessageSerialiser.Serialise(this.GetWelcomeMessage(newUser.Nickname, newUser.MessageColour));
            messageParser.ParseAndSend(welcomeMessageData);
            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, $"Sent welcome message to \"{newUser.Nickname}\"."));
        }

        /// <summary>
        /// Checks which serialiser is suited and deserialises the previously parsed data.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void MessageParser_MessageParsed(object sender, MessageParsedEventArgs e)
        {
            if (ClientMessageSerialiser.CanDeserialise(e.Data))
            {
                var clientMessage = ClientMessageSerialiser.Deserialise(e.Data);
                this.Process(clientMessage, e.EnhancedTcpClient, e.Timestamp);
            }
            else if (ChangeColourMessageSerialiser.CanDeserialise(e.Data))
            {
                var changeColourMessage = ChangeColourMessageSerialiser.Deserialise(e.Data);
                this.Process(changeColourMessage, e.EnhancedTcpClient);
            }
            else if (ChangeNicknameMessageSerialiser.CanDeserialise(e.Data))
            {
                var changeNicknameMessage = ChangeNicknameMessageSerialiser.Deserialise(e.Data);
                this.Process(changeNicknameMessage, e.EnhancedTcpClient);
            }
            else if (ExitChatMessageSerialiser.CanDeserialise(e.Data))
            {
                var exitChatMessage = ExitChatMessageSerialiser.Deserialise(e.Data);
                this.Process(exitChatMessage, e.EnhancedTcpClient);
            }
            else
            {
                this.ActionLogged?.Invoke(
                    this,
                    new ActionLoggedEventArgs(DateTime.Now, "Unknown message type received."));
            }
        }

        /// <summary>
        /// Distributes the received <see cref="ClientMessage"/> to all clients.
        /// </summary>
        /// <param name="message">The received <see cref="ClientMessage"/>.</param>
        /// <param name="senderClient">The sender of the <see cref="ClientMessage"/>.</param>
        /// <param name="timestamp">The timestamp, on which the <see cref="ClientMessage"/> was received.</param>
        private void Process(ClientMessage message, EnhancedTcpClient senderClient, DateTime timestamp)
        {
            var senderEndpoint = senderClient.Client.Client.RemoteEndPoint;
            User messageSender = this.users.Find(x => x.EndPoint == senderEndpoint.ToString());
            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, $"Client \"{messageSender.Nickname}\" sent a message."));
            var serverMessage = new ServerMessage(timestamp, messageSender.Nickname, message.Colour, message.Content);
            var data = ServerMessageSerialiser.Serialise(serverMessage);

            foreach (var parser in this.messageParser)
            {
                parser.ParseAndSend(data);
            }

            string logText = $"Sent message from \"{messageSender.Nickname}\" to all connected clients.";

            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, logText));
        }

        /// <summary>
        /// Checks if the change colour request is allowed and decides how to proceed.
        /// </summary>
        /// <param name="message">The received <see cref="ChangeColourMessage"/>.</param>
        /// <param name="senderClient">The client that sent the <see cref="ChangeColourMessage"/>.</param>
        private void Process(ChangeColourMessage message, EnhancedTcpClient senderClient)
        {
            var senderEndpoint = senderClient.Client.Client.RemoteEndPoint;
            User messageSender = this.users.Find(x => x.EndPoint == senderEndpoint.ToString());

            if (message.Content.ToLower() == "green")
            {
                this.LogColourChange(false, messageSender.Id, messageSender.MessageColour, message.Content);
            }
            else if (message.Content == messageSender.MessageColour)
            {
                this.LogColourChange(false, messageSender.Id, messageSender.MessageColour, message.Content);
            }
            else
            {
                this.ExecuteChangeColourCommand(senderClient, messageSender, message);
            }
        }

        /// <summary>
        /// Checks if the change nickname request is allowed and decides how to proceed.
        /// </summary>
        /// <param name="message">The received <see cref="ChangeNicknameMessage"/>.</param>
        /// <param name="senderClient">The client that sent the <see cref="ChangeNicknameMessage"/>.</param>
        private void Process(ChangeNicknameMessage message, EnhancedTcpClient senderClient)
        {
            var senderEndpoint = senderClient.Client.Client.RemoteEndPoint;
            User messageSender = this.users.Find(x => x.EndPoint == senderEndpoint.ToString());

            if (messageSender.Nickname == message.Content)
            {
                this.LogNicknameChange(false, messageSender.Id, messageSender.Nickname, message.Content);
                return;
            }
            else if (this.users.Exists(x => x.Nickname == message.Content))
            {
                this.LogNicknameChange(false, messageSender.Id, messageSender.Nickname, message.Content);
                return;
            }

            this.ExecuteChangeNicknameCommand(senderClient, messageSender, message);
        }

        /// <summary>
        /// Processes an <see cref="ExitChatMessage"/>. Confirms to the requesting client,
        /// shuts the requesting clients connection down and removes it from the list.
        /// Notifies all connected clients about the user that left.
        /// </summary>
        /// <param name="message">The received <see cref="ExitChatMessage"/>.</param>
        /// <param name="senderClient">The client that sent the <see cref="ExitChatMessage"/>.</param>
        private void Process(ExitChatMessage message, EnhancedTcpClient senderClient)
        {
            if (message.Content != "ExitChat_requested.")
            {
                string logText = "Exit Chat Message with invalid content received.";
                this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, logText));
                return;
            }

            var senderEndpoint = senderClient.Client.Client.RemoteEndPoint;
            var leavingUser = this.users.Find(x => x.EndPoint == senderEndpoint.ToString());

            // Shut everything related to the requesting user down, and remove it from the list.
            var requester = this.messageParser.Find(x => x.EnhancedTcpClient == senderClient);
            requester.ParseAndSend(ExitChatMessageSerialiser.Serialise(new ExitChatMessage("OK")));
            this.messageParser.Remove(requester);
            senderClient.StopAliveMonitoring();
            senderClient.StopDataMonitoring();
            senderClient.Client.Close();
            this.clients.Remove(senderClient);
            this.users.Remove(leavingUser);

            // Notify everyone else.
            var userUpdateMessage = new UserUpdateMessage("left", leavingUser);
            var data = UserUpdateMessageSerialiser.Serialise(userUpdateMessage);
            this.messageParser.ForEach(x => x.ParseAndSend(data));
            this.NotifyAllClients($"\"{leavingUser.Nickname}\" exited the chat.");

            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, "Client exited with command."));
        }

        /// <summary>
        /// Changes the colour of the requesting <see cref="User"/> and notifies all clients appropriately.
        /// </summary>
        /// <param name="senderClient">The client that sent the <see cref="ChangeColourMessage"/>.</param>
        /// <param name="messageSender">The <see cref="User"/> that sent the <see cref="ChangeColourMessage"/>.</param>
        /// <param name="message">The received <see cref="ChangeColourMessage"/>.</param>
        private void ExecuteChangeColourCommand(EnhancedTcpClient senderClient, User messageSender, ChangeColourMessage message)
        {
            var previousColour = messageSender.MessageColour;
            messageSender.MessageColour = message.Content;
            this.LogColourChange(true, messageSender.Id, previousColour, messageSender.MessageColour);

            // Notify requesting client.
            var userUpdateMessage = new UserUpdateMessage("local", messageSender);
            var requester = this.messageParser.Find(x => x.EnhancedTcpClient == senderClient);
            requester.ParseAndSend(UserUpdateMessageSerialiser.Serialise(userUpdateMessage));

            // Notify other clients. 
            userUpdateMessage = new UserUpdateMessage("remote", messageSender);
            var otherClientsParser = this.messageParser.Where(x => x.EnhancedTcpClient != senderClient);
            foreach (var client in otherClientsParser)
            {
                client.ParseAndSend(UserUpdateMessageSerialiser.Serialise(userUpdateMessage));
            }

            this.NotifyAllClients($"\"{messageSender.Nickname}\" changed colour from \"{previousColour}\" " +
                                  $"to \"{messageSender.MessageColour}\".");
        }

        /// <summary>
        /// Changes the nickname of the requesting <see cref="User"/> and notifies all clients appropriately.
        /// </summary>
        /// <param name="senderClient">The client that sent the <see cref="ChangeNicknameMessage"/>.</param>
        /// <param name="messageSender">The <see cref="User"/> that sent the <see cref="ChangeNicknameMessage"/>.</param>
        /// <param name="message">The received <see cref="ChangeNicknameMessage"/>.</param>
        private void ExecuteChangeNicknameCommand(EnhancedTcpClient senderClient, User messageSender, ChangeNicknameMessage message)
        {
            var previousNickname = messageSender.Nickname;
            messageSender.Nickname = message.Content;
            this.LogNicknameChange(true, messageSender.Id, previousNickname, messageSender.Nickname);

            // Notify requesting client.
            var userUpdateMessage = new UserUpdateMessage("local", messageSender);
            var requester = this.messageParser.Find(x => x.EnhancedTcpClient == senderClient);
            requester.ParseAndSend(UserUpdateMessageSerialiser.Serialise(userUpdateMessage));

            // Notify other clients. 
            userUpdateMessage = new UserUpdateMessage("remote", messageSender);
            var otherClientsParser = this.messageParser.Where(x => x.EnhancedTcpClient != senderClient);
            foreach (var client in otherClientsParser)
            {
                client.ParseAndSend(UserUpdateMessageSerialiser.Serialise(userUpdateMessage));
            }

            this.NotifyAllClients($"Nickname changed from \"{previousNickname}\" " +
                                  $"to \"{messageSender.Nickname}\".");
        }

        /// <summary>
        /// Notifies all connected clients by sending them a <see cref="ServerMessage"/>
        /// with the specified text as its content.
        /// </summary>
        /// <param name="text">The notification text for all connected clients.</param>
        private void NotifyAllClients(string text)
        {
            ServerMessage message = new ServerMessage(DateTime.Now, "Server", "Green", text);
            var data = ServerMessageSerialiser.Serialise(message);
            this.messageParser.ForEach(x => x.ParseAndSend(data));

            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, $"Notified all clients about \"{text}\""));
        }

        /// <summary>
        /// Evaluates the next pressed key of the <see cref="KeyboardWatcher"/>.
        /// Tells the <see cref="ServerApplication"/> to shutdown, if the
        /// pressed key was <see cref="ConsoleKey.Escape"/>.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void KeyboardWatcher_OnKeyPressed(object sender, OnKeyPressedEventArgs e)
        {
            if (e.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                this.isActive = false;
            }
        }

        /// <summary>
        /// Builds a welcome message using specified nickname and message colour.
        /// </summary>
        /// <param name="nickname">The specified nickname.</param>
        /// <param name="messageColour">The specified message colour.</param>
        /// <returns>The welcome message as a <see cref="ServerMessage"/>.</returns>
        private ServerMessage GetWelcomeMessage(string nickname, string messageColour)
        {
            string welcomeMessage = "\n Welcome to the ChatServer! \n \n " +
                                    $"Your current nickname is \"{nickname}\". \n " +
                                    $"Your current color is \"{messageColour}\". \n \n " +
                                    "Type \"nickname YOUR_NEW_NICKNAME\" to change your nickname. \n " +
                                    "Type \"color YOUR_NEW_COLOR\" to change your color. \n " +
                                    "Type \"get out\" to exit this chat application. \n";

            return new ServerMessage(DateTime.Now, "Server", "Green", welcomeMessage);
        }

        /// <summary>
        /// Gets the port from the user.
        /// </summary>
        /// <param name="prompt">The prompt message to be displayed to the user.</param>
        /// <returns>The port entered from the user.</returns>
        private int GetPort(string prompt)
        {
            bool isValid;
            int input;
            do
            {
                Console.Write(prompt);
                isValid = int.TryParse(Console.ReadLine(), out input);
                if (input < IPEndPoint.MinPort || input > IPEndPoint.MaxPort)
                {
                    isValid = false;
                }

                if (!isValid)
                {
                    Console.WriteLine($"The specified port must be in between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}, including borders.");
                }
            }
            while (!isValid);

            return input;
        }

        /// <summary>
        /// Logs the colour change action with the specified outcome.
        /// </summary>
        /// <param name="wasSuccessful">The outcome deciding parameter.</param>
        /// <param name="userId">The user id of the requester.</param>
        /// <param name="previousColour">The previous colour of the requester.</param>
        /// <param name="desiredColour">The desired colour of the requester.</param>
        private void LogColourChange(bool wasSuccessful, int userId, string previousColour, string desiredColour)
        {
            string outcome = wasSuccessful ? "Request granted and applied." : "Request denied.";

            string logText = $"Client with ID {userId} wants to change the " +
                             $"colour from \"{previousColour}\" to \"{desiredColour}\". " +
                             $"{outcome}";

            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, logText));
        }

        /// <summary>
        /// Logs the nickname change action with the specified outcome.
        /// </summary>
        /// <param name="wasSuccessful">The outcome deciding parameter.</param>
        /// <param name="userId">The user id of the requester.</param>
        /// <param name="previousNickname">The previous nickname of the requester.</param>
        /// <param name="desiredNickname">The desired nickname of the requester.</param>
        private void LogNicknameChange(bool wasSuccessful, int userId, string previousNickname, string desiredNickname)
        {
            string outcome = wasSuccessful ? "Request granted and applied." : "Request denied.";

            string logText = $"Client with ID \"{userId}\" wants to change the " +
                             $"name from \"{previousNickname}\", to \"{desiredNickname}\". " +
                             $"{outcome}";

            this.ActionLogged?.Invoke(this, new ActionLoggedEventArgs(DateTime.Now, logText));
        }
    }
}
