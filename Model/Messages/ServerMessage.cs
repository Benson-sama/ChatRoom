//-----------------------------------------------------------------
// <copyright file="ServerMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ServerMessage class.</summary>
//-----------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    using System;

    /// <summary>
    /// Represents the <see cref="ServerMessage"/> class. This class is used
    /// to distribute displayable messages among clients.
    /// </summary>
    public class ServerMessage : ClientMessage
    {
        /// <summary>
        /// The name of the sender of this <see cref="ServerMessage"/>.
        /// </summary>
        private string sender;

        /// <summary>
        /// Initialises a new instance of the <see cref="ServerMessage"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp of the <see cref="ServerMessage"/>.</param>
        /// <param name="sender">The name of the sender of the <see cref="ServerMessage"/>.</param>
        /// <param name="colour">The colour of the <see cref="ServerMessage"/>.</param>
        /// <param name="text">The text content of the <see cref="ServerMessage"/>.</param>
        public ServerMessage(DateTime timestamp, string sender,  string colour, string text) : base(colour, text)
        {
            this.Timestamp = timestamp;
            this.Sender = sender;
        }

        /// <summary>
        /// Gets the timestamp of this <see cref="ServerMessage"/>.
        /// </summary>
        /// <value>The timestamp of this <see cref="ServerMessage"/>.</value>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets the name of the sender of this <see cref="ServerMessage"/>.
        /// </summary>
        /// <value>The name of the sender of this <see cref="ServerMessage"/>.</value>
        public string Sender
        {
            get
            {
                return this.sender;
            }

            private set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be empty.");
                }

                this.sender = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
