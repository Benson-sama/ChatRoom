//-------------------------------------------------------------
// <copyright file="MenuEntry.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the MenuEntry class.</summary>
//-------------------------------------------------------------
namespace ChatRoom.Model.ApplicationMenu
{
    using System;

    /// <summary>
    /// Represents the <see cref="MenuEntry"/> class.
    /// </summary>
    public class MenuEntry
    {
        /// <summary>
        /// The title of the menu entry.
        /// </summary>
        private string title;

        /// <summary>
        /// Initialises a new instance of the <see cref="MenuEntry"/> class.
        /// </summary>
        /// <param name="title">The title of the menu entry.</param>
        public MenuEntry(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the title of the menu entry.
        /// </summary>
        /// <value>The title of the menu entry.</value>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be empty.");
                }

                this.title = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
