//-----------------------------------------------------------
// <copyright file="Message.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Message class.</summary>
//-----------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    using System;

    /// <summary>
    /// Represents the <see cref="Message"/> class.
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// The content of the <see cref="Message"/>.
        /// </summary>
        private string content;

        /// <summary>
        /// Initialises a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="text">The text content of the <see cref="Message"/>.</param>
        public Message(string text)
        {
            this.Content = text;
        }

        /// <summary>
        /// Gets the text content of the <see cref="Message"/>.
        /// </summary>
        /// <value>The text content of the <see cref="Message"/>.</value>
        public string Content
        {
            get
            {
                return this.content;
            }

            private set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be empty.");
                }

                this.content = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
