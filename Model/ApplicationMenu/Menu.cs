//-----------------------------------------------------------
// <copyright file="Menu.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Menu class.</summary>
//-----------------------------------------------------------
namespace ChatRoom.Model.ApplicationMenu
{
    using System;

    /// <summary>
    /// Represents the <see cref="Menu"/> class.
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// The index of the currently selected menu entry of the console menu.
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// The menu entries of the console menu.
        /// </summary>
        private MenuEntry[] menuEntries;

        /// <summary>
        /// Initialises a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            this.currentIndex = 0;
            this.menuEntries = new MenuEntry[3]
            {
                new MenuEntry("Start Chatserver"),
                new MenuEntry("Start Chatclient"),
                new MenuEntry("Exit Application")
            };
        }

        /// <summary>
        /// Gets the menu entries of the consoles menu.
        /// </summary>
        /// <value>The menu entries of the consoles menu.</value>
        public MenuEntry[] MenuEntries
        {
            get
            {
                return this.menuEntries;
            }

            private set
            {
                this.menuEntries = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }

        /// <summary>
        /// Gets the index of the currently selected menu entry.
        /// </summary>
        /// <value>The index of the currently selected menu entry.</value>
        public int CurrentIndex
        {
            get
            {
                return this.currentIndex;
            }

            private set
            {
                if (value < 0 || value >= this.menuEntries.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"The specified value must be greater than or equal to 0 and less than {this.MenuEntries.Length}.");
                }

                this.currentIndex = value;
            }
        }

        /// <summary>
        /// Navigates the selection of the consoles menu entry up, if possible.
        /// </summary>
        public void NavigateUp()
        {
            if (this.CurrentIndex > 0)
            {
                this.CurrentIndex--;
            }
        }

        /// <summary>
        /// Navigates the selection of the consoles menu entry down, if possible.
        /// </summary>
        public void NavigateDown()
        {
            if (this.CurrentIndex < this.MenuEntries.Length - 1)
            {
                this.CurrentIndex++;
            }
        }
    }
}
