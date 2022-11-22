//--------------------------------------------------------------------
// <copyright file="ChatRoomRenderer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ChatRoomRenderer class.</summary>
//--------------------------------------------------------------------
namespace ChatRoom.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using ChatRoom.Model;
    using ChatRoom.Model.ApplicationMenu;
    using ChatRoom.Model.Applications;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="ChatRoomRenderer"/> class.
    /// </summary>
    public class ChatRoomRenderer : Renderer
    {
        /// <summary>
        /// The locking object of the renderer, used when printing on the console.
        /// </summary>
        private readonly object consoleLocker;

        /// <summary>
        /// Initialises a new instance of the <see cref="ChatRoomRenderer"/> class.
        /// </summary>
        public ChatRoomRenderer()
        {
            this.consoleLocker = new object();
        }

        /// <summary>
        /// Prints the log message of the <see cref="ServerApplication"/> in the console.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="ActionLoggedEventArgs"/> of the event.</param>
        public void ServerApplication_ActionLogged(object sender, ActionLoggedEventArgs e)
        {
            Console.WriteLine(e.Timestamp + " " + e.Message);
        }

        /// <summary>
        /// Draws the client's graphical user interface on the console window. This method adapts to the console's window size.
        /// </summary>
        public void DrawClientUserInterface()
        {
            Console.Clear();

            this.PrintClientUserInterfaceLines();
            this.PrintClientUserInterfaceBranches();
            this.PrintClientUserInterfaceHeadings();

            Console.SetCursorPosition(3, Console.WindowHeight - 2);
        }

        /// <summary>
        /// Prints the information of the given endpoint in their relative location.
        /// </summary>
        /// <param name="endPoint">The specified endpoint.</param>
        public void PrintEndPointInformation(IPEndPoint endPoint)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;

                // Print the desired endpoint information.
                Console.SetCursorPosition(Console.WindowWidth - 34, Console.WindowHeight - 5);
                Console.Write($"EndPoint: {endPoint}");
                Console.SetCursorPosition(Console.WindowWidth - 31, Console.WindowHeight - 4);
                Console.Write("State: ");

                // Reset changes to the consoles parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }

        /// <summary>
        /// Prints the connected status in its corresponding location.
        /// </summary>
        public void PrintConnectedStatus()
        {
            this.PrintLineInColorAtLocation((Console.WindowWidth - 34) + 10, Console.WindowHeight - 4, ConsoleColor.Green, ConsoleColor.Black, "Connected");
        }

        /// <summary>
        /// Prints the disconnected status in its corresponding location.
        /// </summary>
        public void PrintDisconnectedStatus()
        {
            this.PrintLineInColorAtLocation((Console.WindowWidth - 34) + 10, Console.WindowHeight - 4, ConsoleColor.Red, ConsoleColor.Black, "Disconnected");
        }

        /// <summary>
        /// Prints the current user input, printing only the last number of elements that can fit into the user input area.
        /// </summary>
        /// <param name="input">The current input of the user as a string.</param>
        /// <param name="colour">The colour of the input of the user that matches a <see cref="ConsoleColor"/>, case insensitive.</param>
        public void PrintInputinColour(string input, string colour)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                ConsoleColor previousBackgroundColour = Console.BackgroundColor;
                ConsoleColor previousForegroundColour = Console.ForegroundColor;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = this.Convert(colour);
                int maximumInputLength = Console.WindowWidth - 5;

                if (input.Length <= maximumInputLength)
                {
                    this.PrintLineAtLocation(3, Console.WindowHeight - 2, input);
                }
                else
                {
                    // Only print the last number of elements that can fit into one console row.
                    this.PrintLineAtLocation(3, Console.WindowHeight - 2, input.Substring(input.Length - maximumInputLength));
                }

                // Reset changes to the consoles parameters.
                Console.BackgroundColor = previousBackgroundColour;
                Console.ForegroundColor = previousForegroundColour;
            }
        }

        /// <summary>
        /// Prints the user list at the corresponding area of the user interface.
        /// </summary>
        /// <param name="participatingUsers">The specified list of users to be printed.</param>
        public void Print(List<User> participatingUsers)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;
                ConsoleColor previousForegroundColour = Console.ForegroundColor;
                ConsoleColor previousBackgroundColour = Console.BackgroundColor;

                this.ClearClientsArea();

                // Print the user list.
                Console.BackgroundColor = ConsoleColor.Black;
                for (int i = 0; i < participatingUsers.Count; i++)
                {
                    if (i > Console.WindowHeight - 12)
                    {
                        break;
                    }

                    Console.SetCursorPosition(Console.WindowWidth - 36, 3 + i);
                    Console.Write(participatingUsers[i].EndPoint);
                    Console.SetCursorPosition(Console.WindowWidth - 14, 3 + i);
                    Console.Write("| ");
                    ConsoleColor userColour = this.Convert(participatingUsers[i].MessageColour);
                    Console.ForegroundColor = userColour != ConsoleColor.Black ? userColour : ConsoleColor.White;
                    string nickname = participatingUsers[i].Nickname;
                    nickname = (nickname.Length <= 10) ? nickname : new string(nickname.Take(10).ToArray());
                    Console.Write(nickname);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                // Restore previous console parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
                Console.ForegroundColor = previousForegroundColour;
                Console.BackgroundColor = previousBackgroundColour;
            }
        }

        /// <summary>
        /// Prints the list of server messages, showing only the last messages that
        /// fit into the messages area.
        /// </summary>
        /// <param name="serverMessages">The server messages to be printed.</param>
        public void Print(List<ServerMessage> serverMessages)
        {
            if (!serverMessages.Any())
            {
                return;
            }

            lock (this.consoleLocker)
            {
                this.ClearMessagesArea();
                Console.SetCursorPosition(2, 3);
                List<ServerMessage> filteredMessages = new List<ServerMessage>();
                int rowCount = 0;
                int maxRowCount = Console.WindowHeight - 6;

                for (int i = serverMessages.Count - 1; i >= 0; i--)
                {
                    rowCount += this.GetRowCount(serverMessages[i]);

                    if (rowCount < maxRowCount)
                    {
                        filteredMessages.Add(serverMessages[i]);
                    }
                    else
                    {
                        break;
                    }
                }

                filteredMessages.Reverse();
                filteredMessages.ForEach(x => 
                {
                    this.Print(x);
                    Console.SetCursorPosition(2, Console.CursorTop + 1);
                });

                Console.SetCursorPosition(3, Console.WindowHeight - 2);
            }
        }

        /// <summary>
        /// Clears the user input area.
        /// </summary>
        public void ClearInput()
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;

                // Clear the user interface input area.
                Console.SetCursorPosition(3, Console.WindowHeight - 2);
                for (int i = 0; i < Console.WindowWidth - 5; i++)
                {
                    Console.Write(" ");
                }

                // Reset changes to the consoles parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }

        /// <summary>
        /// Prints the straight lines of the graphical user interface in their relative positions.
        /// </summary>
        private void PrintClientUserInterfaceLines()
        {
            for (int i = 1; i < Console.WindowWidth - 1; i++)
            {
                this.PrintCharacterAtPositionIfPossible(i, 0, '═');
                this.PrintCharacterAtPositionIfPossible(i, Console.WindowHeight - 1, '═');
                this.PrintCharacterAtPositionIfPossible(i, Console.WindowHeight - 3, '─');
            }

            for (int i = 1; i < Console.WindowWidth - 38; i++)
            {
                this.PrintCharacterAtPositionIfPossible(i, 2, '─');
            }

            for (int i = Console.WindowWidth - 37; i < Console.WindowWidth - 1; i++)
            {
                this.PrintCharacterAtPositionIfPossible(i, Console.WindowHeight - 8, '─');
            }

            for (int i = 1; i < Console.WindowHeight - 1; i++)
            {
                this.PrintCharacterAtPositionIfPossible(0, i, '║');
                this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 1, i, '║');
            }

            for (int i = 1; i < Console.WindowHeight - 3; i++)
            {
                this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 38, i, '│');
            }
        }

        /// <summary>
        /// Prints the branches of the graphical user interface in their relative positions.
        /// </summary>
        private void PrintClientUserInterfaceBranches()
        {
            this.PrintCharacterAtPositionIfPossible(0, 0, '╔');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 1, 0, '╗');
            this.PrintCharacterAtPositionIfPossible(0, Console.WindowHeight - 1, '╚');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 1, Console.WindowHeight - 1, '╝');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 38, 0, '╤');
            this.PrintCharacterAtPositionIfPossible(0, 2, '╟');
            this.PrintCharacterAtPositionIfPossible(0, Console.WindowHeight - 3, '╟');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 1, Console.WindowHeight - 8, '╢');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 1, Console.WindowHeight - 3, '╢');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 38, 2, '┤');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 38, Console.WindowHeight - 8, '├');
            this.PrintCharacterAtPositionIfPossible(Console.WindowWidth - 38, Console.WindowHeight - 3, '┴');
        }

        /// <summary>
        /// Prints the headings of the graphical user interface in their relative positions.
        /// </summary>
        private void PrintClientUserInterfaceHeadings()
        {
            this.PrintLineAtLocation(2, 1, "Messages");
            this.PrintLineAtLocation(Console.WindowWidth - 36, 1, "Clients");
            this.PrintLineAtLocation(Console.WindowWidth - 36, 2, "───────");
            this.PrintLineAtLocation(Console.WindowWidth - 36, Console.WindowHeight - 7, "Server");
            this.PrintLineAtLocation(Console.WindowWidth - 36, Console.WindowHeight - 6, "──────");
            this.PrintCharacterAtPositionIfPossible(1, Console.WindowHeight - 2, '>');
        }

        /// <summary>
        /// Calculates the row count of the <see cref="ServerMessage"/>.
        /// </summary>
        /// <param name="serverMessage">The given <see cref="ServerMessage"/>.</param>
        /// <returns>The number of rows that the given <see cref="ServerMessage"/> requires.</returns>
        private int GetRowCount(ServerMessage serverMessage)
        {
            int rowCount = 1;
            int currentPosition = 0;

            int displayableAreaWidth = Console.WindowWidth - 41;
            string messageSender = serverMessage.Sender.LimitToNumber(10, true);
            int lengthOfMessageSender = messageSender.Length;
            int messageContentLimit = (8 * displayableAreaWidth) - lengthOfMessageSender - 27;

            string messageText = serverMessage.Content.LimitToNumber(messageContentLimit, true);
            string[] wordsOfMessageText = messageText.Split(' ');

            currentPosition += $"{serverMessage.Timestamp} {messageSender} says: )".Length;

            foreach (var word in wordsOfMessageText)
            {
                if (word == "\n")
                {
                    rowCount++;
                    currentPosition = 0;
                    continue;
                }

                if (displayableAreaWidth - (Console.CursorLeft - 2) < word.Length)
                {
                    rowCount++;
                    currentPosition = 0;
                }

                foreach (var character in word)
                {
                    if (currentPosition >= displayableAreaWidth)
                    {
                        rowCount++;
                        currentPosition = 0;
                        continue;
                    }

                    currentPosition++;
                }
            }

            return rowCount;
        }

        /// <summary>
        /// Prints the given <see cref="ServerMessage"/> at the current location.
        /// </summary>
        /// <param name="serverMessage">The given <see cref="ServerMessage"/>.</param>
        private void Print(ServerMessage serverMessage)
        {
            ConsoleColor previousBackgroundColour = Console.BackgroundColor;
            ConsoleColor previousForegroundColour = Console.ForegroundColor;

            int displayableAreaWidth = Console.WindowWidth - 41;
            string messageSender = serverMessage.Sender.LimitToNumber(10, true);
            int lengthOfMessageSender = messageSender.Length;
            int messageContentLimit = (8 * displayableAreaWidth) - lengthOfMessageSender - 27;

            string messageText = serverMessage.Content.LimitToNumber(messageContentLimit, true);
            string[] wordsOfMessageText = messageText.Split(' ');

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{serverMessage.Timestamp} {messageSender} says: ");

            ConsoleColor messageColour = this.Convert(serverMessage.Colour);
            Console.ForegroundColor = messageColour != ConsoleColor.Black ? messageColour : ConsoleColor.White;

            foreach (var word in wordsOfMessageText)
            {
                if (word == "\n" && Console.CursorTop < Console.WindowHeight - 1)
                {
                    Console.SetCursorPosition(2, Console.CursorTop + 1);
                    continue;
                }

                if (displayableAreaWidth - (Console.CursorLeft - 2) < word.Length)
                {
                    Console.SetCursorPosition(2, Console.CursorTop + 1);
                }

                foreach (var character in word)
                {
                    if (Console.CursorLeft >= Console.WindowWidth - 38)
                    {
                        Console.SetCursorPosition(2, Console.CursorTop + 1);
                    }

                    Console.Write(character);
                }

                Console.Write(" ");
            }

            Console.BackgroundColor = previousBackgroundColour;
            Console.ForegroundColor = previousForegroundColour;
        }

        /// <summary>
        /// Clears the entire area for the connected clients.
        /// </summary>
        private void ClearClientsArea()
        {
            for (int i = 3; i < Console.WindowHeight - 8; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 37, i);
                for (int j = 0; j < 36; j++)
                {
                    Console.Write(" ");
                }
            }
        }

        /// <summary>
        /// Clears the entire area for the received messages.
        /// </summary>
        private void ClearMessagesArea()
        {
            for (int i = 3; i < Console.WindowHeight - 3; i++)
            {
                Console.SetCursorPosition(1, i);
                for (int j = 1; j < (Console.WindowWidth - 38); j++)
                {
                    Console.Write(" ");
                }
            }
        }

        /// <summary>
        /// Converts a string to its equivalent <see cref="ConsoleColor"/>. This conversion is case insensitive.
        /// </summary>
        /// <param name="colour">The colour as a string to be converted.</param>
        /// <returns>The resulting <see cref="ConsoleColor"/> of the conversion.
        /// <see cref="ConsoleColor.Gray"/> if no matching <see cref="ConsoleColor"/> was found.</returns>
        private ConsoleColor Convert(string colour)
        {
            switch (colour.ToLower())
            {
                case "black":
                    return ConsoleColor.Black;
                case "darkblue":
                    return ConsoleColor.DarkBlue;
                case "darkgreen":
                    return ConsoleColor.DarkGreen;
                case "darkcyan":
                    return ConsoleColor.DarkCyan;
                case "darkred":
                    return ConsoleColor.DarkRed;
                case "darkmagenta":
                    return ConsoleColor.DarkMagenta;
                case "darkyellow":
                    return ConsoleColor.DarkYellow;
                case "gray":
                default:
                    return ConsoleColor.Gray;
                case "darkgray":
                    return ConsoleColor.DarkGray;
                case "blue":
                    return ConsoleColor.Blue;
                case "green":
                    return ConsoleColor.Green;
                case "cyan":
                    return ConsoleColor.Cyan;
                case "red":
                    return ConsoleColor.Red;
                case "magenta":
                    return ConsoleColor.Magenta;
                case "yellow":
                    return ConsoleColor.Yellow;
                case "white":
                    return ConsoleColor.White;
            }
        }
    }
}
