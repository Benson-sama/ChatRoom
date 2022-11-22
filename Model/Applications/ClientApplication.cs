//---------------------------------------------------------------------
// <copyright file="ClientApplication.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ClientApplication class.</summary>
//---------------------------------------------------------------------
namespace ChatRoom.Model.Applications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using ChatRoom.Model.EnhancedTCPC;
    using ChatRoom.Model.Messages;
    using ChatRoom.Model.Serialiser;
    using ChatRoom.View;

    /// <summary>
    /// Represents the <see cref="ClientApplication"/> class.
    /// </summary>
    public class ClientApplication
    {
        /// <summary>
        /// The list of <see cref="ServerMessage"/>s of this <see cref="ClientApplication"/>.
        /// </summary>
        private readonly List<ServerMessage> messages;

        /// <summary>
        /// The <see cref="EnhancedTcpClient"/> of this <see cref="ClientApplication"/>.
        /// </summary>
        private EnhancedTcpClient server;

        /// <summary>
        /// The <see cref="MessageParser"/> of this <see cref="ClientApplication"/>.
        /// </summary>
        private MessageParser messageParser;

        /// <summary>
        /// The <see cref="ChatRoom.View.Renderer"/> of this <see cref="ClientApplication"/>.
        /// </summary>
        private ChatRoomRenderer renderer;

        /// <summary>
        /// The local <see cref="User"/> of this <see cref="ClientApplication"/>.
        /// </summary>
        private User localUser;

        /// <summary>
        /// The participating <see cref="User"/>s of this <see cref="ClientApplication"/>.
        /// </summary>
        private List<User> participatingUsers;

        /// <summary>
        /// The connection status of this <see cref="ClientApplication"/>.
        /// </summary>
        private bool isConnectedToServer;

        /// <summary>
        /// The value indicating whether the <see cref="localUser"/> wants to
        /// exit this <see cref="ClientApplication"/>.
        /// </summary>
        private bool isExitChatRequested;

        /// <summary>
        /// Initialises a new instance of the <see cref="ClientApplication"/> class.
        /// </summary>
        /// <param name="renderer">The <see cref="ChatRoom.View.Renderer"/> instance for this <see cref="ClientApplication"/>.</param>
        public ClientApplication(ChatRoomRenderer renderer)
        {
            this.Renderer = renderer;
            this.messages = new List<ServerMessage>();
            this.participatingUsers = new List<User>();
        }

        /// <summary>
        /// Gets the <see cref="ChatRoom.View.Renderer"/> of this <see cref="ClientApplication"/>.
        /// </summary>
        /// <value>The <see cref="ChatRoom.View.Renderer"/> of this <see cref="ClientApplication"/>.</value>
        public ChatRoomRenderer Renderer
        {
            get
            {
                return this.renderer;
            }

            private set
            {
                this.renderer = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }

        /// <summary>
        /// Starts this <see cref="ClientApplication"/>.
        /// </summary>
        public void Run()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            while (Console.WindowHeight < 20 || Console.WindowWidth < 80)
            {
                Console.WriteLine("The console window must be at least 80 wide and 20 high.");
                Console.WriteLine("Resize the console window and press any key to try again...");
                Console.ReadKey(true);
            }

            Console.Clear();
            this.EstablishServerConnection();

            while (true)
            {
                string input = this.GetNextUserTextInput();
                this.ProcessLocalUserInput(input);
            }
        }

        /// <summary>
        /// Establishes a server connection. Gets the target endpoint from the user and repeats on failure.
        /// </summary>
        private void EstablishServerConnection()
        {
            while (true)
            {
                IPEndPoint targetServer = this.GetIPEndpoint("To establish a connection to the server an IP-Endpoint is required.");
                Console.WriteLine($"Connecting to {targetServer.Address}:{targetServer.Port}");

                TcpClient server = new TcpClient();

                try
                {
                    server.Connect(targetServer);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Es konnte keine Verbindung zum angegebenen Server hergestellt werden.\n" +
                                      "Bitte stellen Sie sicher, dass der angegebene Endpunkt existiert und erreichbar ist.\n" +
                                      "Bei Schwierigkeiten wenden Sie sich an den Systemadministrator mit folgender Nachricht:\n" +
                                      "\"" + e.Message + "\"\n");
                    continue;
                }

                this.Renderer.DrawClientUserInterface();
                this.Renderer.PrintEndPointInformation(targetServer);
                this.Renderer.PrintConnectedStatus();
                this.SetUpClientApplication(server);

                while (this.localUser == null)
                {
                    Thread.Sleep(50);
                }

                break;
            }
        }

        /// <summary>
        /// Sets up the server and message parser environment of this <see cref="ClientApplication"/>.
        /// </summary>
        /// <param name="server">The server of this <see cref="ClientApplication"/>.</param>
        private void SetUpClientApplication(TcpClient server)
        {
            this.server = new EnhancedTcpClient(server);
            this.messageParser = new MessageParser(this.server, 8);
            this.server.Connected += this.OnConnected;
            this.server.Disconnected += this.OnDisconnected;
            this.server.DataReceived += this.messageParser.Collect;
            this.messageParser.MessageParsed += this.MessageParser_MessageParsed;
            this.server.StartDataMonitoring();
            this.server.StartAliveMonitoring();
        }

        /// <summary>
        /// Evaluates the incoming message data and processes it.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void MessageParser_MessageParsed(object sender, MessageParsedEventArgs e)
        {
            if (ServerMessageSerialiser.CanDeserialise(e.Data))
            {
                var serverMessage = ServerMessageSerialiser.Deserialise(e.Data);
                this.Process(serverMessage);
            }
            else if (UserUpdateMessageSerialiser.CanDeserialise(e.Data))
            {
                var userUpdateMessage = UserUpdateMessageSerialiser.Deserialise(e.Data);
                this.Process(userUpdateMessage);
            }
            else if (this.isExitChatRequested && ExitChatMessageSerialiser.CanDeserialise(e.Data))
            {
                var exitChatMessage = ExitChatMessageSerialiser.Deserialise(e.Data);
                this.Process(exitChatMessage);
            }
            else if (UserListSerialiser.CanDeserialise(e.Data))
            {
                this.participatingUsers = UserListSerialiser.Deserialise(e.Data);
                this.Renderer.Print(this.participatingUsers);
            }
        }

        /// <summary>
        /// Gets the next input string from the user and lets the <see cref="ChatRoom.Renderer"/>
        /// print it in its designated area on change.
        /// </summary>
        /// <returns>The next input string from the user.</returns>
        private string GetNextUserTextInput()
        {
            string input = string.Empty;
            while (true)
            {
                ConsoleKeyInfo nextKeyInfo = Console.ReadKey(true);
                if (nextKeyInfo.Key == ConsoleKey.Enter)
                {
                    if (!this.isConnectedToServer)
                    {
                        Console.Beep(420, 500);
                        continue;
                    }

                    this.Renderer.ClearInput();
                    return input;
                }
                else if (nextKeyInfo.Key == ConsoleKey.Backspace && input.Any())
                {
                    input = input.Remove(input.Length - 1);
                    this.Renderer.ClearInput();
                    this.Renderer.PrintInputinColour(input, this.localUser.MessageColour);
                }
                else if (nextKeyInfo.Key == ConsoleKey.Tab)
                {
                    continue;
                }
                else
                {
                    input += nextKeyInfo.KeyChar;
                    this.Renderer.PrintInputinColour(input, this.localUser.MessageColour);
                }
            }
        }

        /// <summary>
        /// Processes the given input string based on the commands "get out", "nickname" and "colour".
        /// </summary>
        /// <param name="input">The specified input string.</param>
        private void ProcessLocalUserInput(string input)
        {
            if (!input.Any())
            {
                return;
            }

            if (input.StartsWith("get out"))
            {
                this.ExecuteExitClientApplicationCommand();
                return;
            }

            string[] words = input.Split(' ');

            switch (words[0])
            {
                case "nickname":
                    this.ExecuteChangeNicknameCommand(words[1]);
                    break;
                case "color":
                    this.ExecuteChangeColorCommand(words[1]);
                    break;
                default:
                    this.ExecuteSendMessageCommand(input);
                    break;
            }
        }

        /// <summary>
        /// Sends a <see cref="ClientMessage"/> using the
        /// message colour of the local user to the server.
        /// </summary>
        /// <param name="input">The content of the <see cref="ClientMessage"/>.</param>
        private void ExecuteSendMessageCommand(string input)
        {
            var message = new ClientMessage(this.localUser.MessageColour, input);
            var data = ClientMessageSerialiser.Serialise(message);
            this.messageParser.ParseAndSend(data);
        }

        /// <summary>
        /// Sends a <see cref="ChangeNicknameMessage"/> to the server.
        /// </summary>
        /// <param name="nickname">The desired nickname.</param>
        private void ExecuteChangeNicknameCommand(string nickname)
        {
            var message = new ChangeNicknameMessage(nickname);
            var data = ChangeNicknameMessageSerialiser.Serialise(message);
            this.messageParser.ParseAndSend(data);
        }

        /// <summary>
        /// Sends a <see cref="ChangeColourMessage"/> to the server.
        /// </summary>
        /// <param name="colour">The desired colour.</param>
        private void ExecuteChangeColorCommand(string colour)
        {
            var message = new ChangeColourMessage(colour);
            var data = ChangeColourMessageSerialiser.Serialise(message);
            this.messageParser.ParseAndSend(data);
        }

        /// <summary>
        /// Sends an <see cref="ExitChatMessage"/> to the server
        /// and sets the boolean flag of this <see cref="ClientApplication"/>
        /// indicating that this client wants to exit.
        /// That way the client only exits on receiving an <see cref="ExitChatMessage"/>
        /// when it was previously requested.
        /// </summary>
        private void ExecuteExitClientApplicationCommand()
        {
            var exitChatMessage = new ExitChatMessage("ExitChat_requested.");
            var data = ExitChatMessageSerialiser.Serialise(exitChatMessage);
            this.messageParser.ParseAndSend(data);
            this.isExitChatRequested = true;
        }

        /// <summary>
        /// Processes an incoming <see cref="ServerMessage"/>.
        /// Adding it to the list of messages and telling the
        /// <see cref="ChatRoom.View.Renderer"/> to refresh the view.
        /// </summary>
        /// <param name="serverMessage">The incoming <see cref="ServerMessage"/>.</param>
        private void Process(ServerMessage serverMessage)
        {
            this.messages.Add(serverMessage);
            this.Renderer.Print(this.messages);
        }

        /// <summary>
        /// Processes an incoming <see cref="UserUpdateMessage"/> based on its content.
        /// "local": The receiving users information changed.
        /// "remote": A remote user changed either colour or nickname.
        /// "joined": A new remote user joined.
        /// "left": A remote user left.
        /// </summary>
        /// <param name="userUpdateMessage">The incoming <see cref="UserUpdateMessage"/>.</param>
        private void Process(UserUpdateMessage userUpdateMessage)
        {
            switch (userUpdateMessage.Content)
            {
                case "local":
                    this.ExecuteLocalUserUpdateCommand(userUpdateMessage.User);
                    break;
                case "remote":
                    this.ExecuteRemoteUserUpdateCommand(userUpdateMessage.User);
                    break;
                case "joined":
                    this.ExecuteUserJoinedCommand(userUpdateMessage.User);
                    break;
                case "left":
                    this.ExecuteUserLeftCommand(userUpdateMessage.User);
                    break;
                default:
                    // Log: Unknown user update message received.
                    break;
            }
        }

        /// <summary>
        /// Executes the user left command. Removes the user from
        /// the list of participating users of this <see cref="ClientApplication"/>
        /// and refreshes the user interface.
        /// </summary>
        /// <param name="leavingUser">The leaving user.</param>
        private void ExecuteUserLeftCommand(User leavingUser)
        {
            this.participatingUsers.RemoveAll(x => x.Id == leavingUser.Id);
            this.Renderer.Print(this.participatingUsers);
        }

        /// <summary>
        /// Executes the user joined command. Adds the new user
        /// to the list of participating users of this <see cref="ClientApplication"/>
        /// and refreshes the user interface.
        /// </summary>
        /// <param name="newUser">The new user.</param>
        private void ExecuteUserJoinedCommand(User newUser)
        {
            this.participatingUsers.Add(newUser);
            this.Renderer.Print(this.participatingUsers);
        }

        /// <summary>
        /// Executes the remote user update command. Replaces the
        /// user of the list of participating users, whose ID matches
        /// the new users ID, with the new user.
        /// </summary>
        /// <param name="user">The user that has changed.</param>
        private void ExecuteRemoteUserUpdateCommand(User user)
        {
            User outdatedUser = this.participatingUsers.Find(
                x => x.Id == user.Id);

            if (outdatedUser == null)
            {
                // Could not find the specified remote user.
                return;
            }

            this.participatingUsers.Remove(outdatedUser);
            this.participatingUsers.Add(user);
            this.participatingUsers = this.participatingUsers.OrderBy(x => x.Id).ToList();

            this.Renderer.Print(this.participatingUsers);
        }

        /// <summary>
        /// Executes the local user update command. Replaces the local user of
        /// this <see cref="ClientApplication"/> with the specified user.
        /// </summary>
        /// <param name="user">The new user for this <see cref="ClientApplication"/>.</param>
        private void ExecuteLocalUserUpdateCommand(User user)
        {
            this.localUser = user;
        }

        /// <summary>
        /// Processes an incoming <see cref="ExitChatMessage"/>.
        /// The <see cref="ClientApplication"/> will close the network connection and terminate,
        /// if the exit was requested and the message content was "OK".
        /// </summary>
        /// <param name="exitChatMessage">The incoming <see cref="ExitChatMessage"/>.</param>
        private void Process(ExitChatMessage exitChatMessage)
        {
            if (exitChatMessage.Content == "OK")
            {
                this.server.StopAliveMonitoring();
                this.server.StopDataMonitoring();
                this.server.Client.Close();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Sets the connection status flag of this <see cref="ClientApplication"/>.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args of the event.</param>
        private void OnConnected(object sender, ConnectedEventArgs e)
        {
            this.isConnectedToServer = true;
            this.Renderer.PrintConnectedStatus();
        }

        /// <summary>
        /// Sets the connection status flag of this <see cref="ClientApplication"/>.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args of the event.</param>
        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            this.isConnectedToServer = false;
            this.Renderer.PrintDisconnectedStatus();
        }

        /// <summary>
        /// Gets the desired <see cref="IPEndPoint"/> from the user with a specified prompt message.
        /// </summary>
        /// <param name="message">The message to be displayed to the user.</param>
        /// <returns>The <see cref="IPEndPoint"/> with the desired parameters from the user.</returns>
        private IPEndPoint GetIPEndpoint(string message)
        {
            Console.WriteLine(message + "\n");
            IPAddress ipAddress = this.GetIPAddress("Please enter the IP-Address: ");
            int port = this.GetPort("Please enter the port number: ");

            return new IPEndPoint(ipAddress, port);
        }

        /// <summary>
        /// Gets the input IP address from the user.
        /// </summary>
        /// <param name="prompt">The prompt message to be displayed to the user.</param>
        /// <returns>The IP address entered from the user.</returns>
        private IPAddress GetIPAddress(string prompt)
        {
            bool isValid;
            IPAddress input;
            do
            {
                Console.Write(prompt);
                isValid = IPAddress.TryParse(Console.ReadLine(), out input);

                if (!isValid)
                {
                    Console.WriteLine($"The input must be a valid IP address!");
                }
            }
            while (!isValid);

            return input;
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
    }
}
