//-------------------------------------------------------------------
// <copyright file="ConsoleSettings.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ConsoleSettings class.</summary>
//-------------------------------------------------------------------
namespace ChatRoom.Model.ConsoleModel
{
    using System;

    /// <summary>
    /// Represents the <see cref="ConsoleSettings"/> class.
    /// </summary>
    public class ConsoleSettings
    {
        /// <summary>
        /// The window's height of the ConsoleSettings.
        /// </summary>
        private int windowHeight;

        /// <summary>
        /// The window's width of the ConsoleSettings.
        /// </summary>
        private int windowWidth;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConsoleSettings"/> class and automatically captures the console settings at that time.
        /// </summary>
        public ConsoleSettings()
        {
            this.Capture();
        }

        /// <summary>
        /// Gets or sets the foreground colour of the ConsoleSettings.
        /// </summary>
        /// <value>The foreground colour of the ConsoleSettings.</value>
        public ConsoleColor ForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the background colour of the ConsoleSettings.
        /// </summary>
        /// <value>The background colour of the ConsoleSettings.</value>
        public ConsoleColor BackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the window's height of the ConsoleSettings.
        /// </summary>
        /// <value>The window's height of the ConsoleSettings.</value>
        public int WindowHeight
        {
            get
            {
                return this.windowHeight;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be equal or less than 0.");
                }

                this.windowHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the window's width of the ConsoleSettings.
        /// </summary>
        /// <value>The window's width of the ConsoleSettings.</value>
        public int WindowWidth
        {
            get
            {
                return this.windowWidth;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be equal or less than 0.");
                }

                this.windowWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cursor of the ConsoleSettings is visible.
        /// </summary>
        /// <value>The visibility of the cursor of the ConsoleSettings.</value>
        public bool CursorVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Saves the current console settings. This includes window size, colours and cursor visibility.
        /// </summary>
        public void Capture()
        {
            this.ForegroundColor = Console.ForegroundColor;
            this.BackgroundColor = Console.BackgroundColor;
            this.WindowHeight = Console.WindowHeight;
            this.WindowWidth = Console.WindowWidth;
            this.CursorVisible = Console.CursorVisible;
        }

        /// <summary>
        /// Restores the previously captured console settings. This includes window size, colours and cursor visibility.
        /// </summary>
        public void Restore()
        {
            Console.ForegroundColor = this.ForegroundColor;
            Console.BackgroundColor = this.BackgroundColor;
            Console.WindowHeight = this.WindowHeight;
            Console.WindowWidth = this.WindowWidth;
            Console.CursorVisible = this.CursorVisible;
        }
    }
}