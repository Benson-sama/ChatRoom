//------------------------------------------------------------
// <copyright file="Renderer.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Renderer class.</summary>
//------------------------------------------------------------
namespace ChatRoom.View
{
    using System;
    using ChatRoom.Model.ApplicationMenu;

    /// <summary>
    /// Represents the <see cref="Renderer"/> class.
    /// </summary>
    public class Renderer
    {
        /// <summary>
        /// The locking object of the renderer, used when printing on the console.
        /// </summary>
        private readonly object consoleLocker;

        /// <summary>
        /// Initialises a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        public Renderer()
        {
            this.consoleLocker = new object();
        }

        /// <summary>
        /// Draws the console menu on the console window.
        /// </summary>
        /// <param name="consoleMenu">The console menu.</param>
        public void Draw(Menu consoleMenu)
        {
            // Clear a little more than the space of the console menu, considering the user decreases the consoles window size. (May cause console flickering.)
            for (int i = 0; i < consoleMenu.MenuEntries.Length + 1; i++)
            {
                this.ClearSpecificRow(i);
            }

            // Stop if the console menu has no entries.
            if (consoleMenu.MenuEntries.Length < 1)
            {
                return;
            }

            for (int i = 0; i < consoleMenu.MenuEntries.Length; i++)
            {
                string currentMenuEntryTitle = consoleMenu.MenuEntries[i].Title;

                if (consoleMenu.CurrentIndex == i)
                {
                    this.PrintLineInColorAtLocation(0, i, ConsoleColor.Black, ConsoleColor.White, currentMenuEntryTitle);
                }
                else
                {
                    this.PrintLineInColorAtLocation(0, i, ConsoleColor.White, ConsoleColor.Black, currentMenuEntryTitle);
                }
            }
        }

        /// <summary>
        /// Erases a specific console position.
        /// </summary>
        /// <param name="left">The column position of the desired location.</param>
        /// <param name="top">The row position of the desired location.</param>
        public void ClearSpecificConsolePosition(int left, int top)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;

                // Clear the desired position.
                Console.SetCursorPosition(left, top);
                Console.Write(" ");

                // Reset changes to the consoles parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }

        /// <summary>
        /// Clears a specific row of the console's window.
        /// </summary>
        /// <param name="top">The specified row of the console window.</param>
        internal void ClearSpecificRow(int top)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;

                // Clear the desired row.
                Console.SetCursorPosition(0, top);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write(" ");
                }

                // Reset changes to the consoles parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }

        /// <summary>
        /// Only prints the character at the given destination, if it is located inside the console window.
        /// </summary>
        /// <param name="left">The column coordinate of the destination.</param>
        /// <param name="top">The row coordinate of the destination.</param>
        /// <param name="character">The character, that gets printed if it is possible.</param>
        internal void PrintCharacterAtPositionIfPossible(int left, int top, char character)
        {
            if (left < 0 || left >= Console.WindowWidth
              || top < 0 || top >= Console.WindowHeight)
            {
                return;
            }

            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;

                // Print the the desired character.
                Console.SetCursorPosition(left, top);
                Console.Write(character);

                // Reset changes to the consoles parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }

        /// <summary>
        /// Prints a text at a specified location with specified foreground and background colour and restores the previous cursor position.
        /// </summary>
        /// <param name="left">The column position of the text.</param>
        /// <param name="top">The row position of the text.</param>
        /// <param name="foregroundColour">The foreground colour of the text.</param>
        /// <param name="backgroundColour">The background colour of the text.</param>
        /// <param name="text">The text that gets printed.</param>
        internal void PrintLineInColorAtLocation(int left, int top, ConsoleColor foregroundColour, ConsoleColor backgroundColour, string text)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;
                ConsoleColor previousForegroundColour = Console.ForegroundColor;
                ConsoleColor previousBackgroundColour = Console.BackgroundColor;

                // Print the the desired string.
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = foregroundColour;
                Console.BackgroundColor = backgroundColour;
                Console.Write(text);

                // Reset changes to the consoles parameters.
                Console.ForegroundColor = previousForegroundColour;
                Console.BackgroundColor = previousBackgroundColour;
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }

        /// <summary>
        /// Prints a text at a specified location and restores the previous cursor position.
        /// </summary>
        /// <param name="left">The column position of the text.</param>
        /// <param name="top">The row position of the text.</param>
        /// <param name="text">The text that gets printed.</param>
        internal void PrintLineAtLocation(int left, int top, string text)
        {
            lock (this.consoleLocker)
            {
                // Save current console parameters.
                int previousLeft = Console.CursorLeft;
                int previousTop = Console.CursorTop;

                // Print the the desired string.
                Console.SetCursorPosition(left, top);
                Console.Write(text);

                // Reset changes to the consoles parameters.
                Console.SetCursorPosition(previousLeft, previousTop);
            }
        }
    }
}
