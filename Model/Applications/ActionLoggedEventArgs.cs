//-------------------------------------------------------------------------
// <copyright file="ActionLoggedEventArgs.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ActionLoggedEventArgs class.</summary>
//-------------------------------------------------------------------------
namespace ChatRoom.Model.Applications
{
    using System;

    /// <summary>
    /// Represents the <see cref="ActionLoggedEventArgs"/> class.
    /// </summary>
    public class ActionLoggedEventArgs
    {
        /// <summary>
        /// The message of the <see cref="ActionLoggedEventArgs"/>.
        /// </summary>
        private string message;

        /// <summary>
        /// Initialises a new instance of the <see cref="ActionLoggedEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp of the <see cref="ActionLoggedEventArgs"/>.</param>
        /// <param name="message">The message of the <see cref="ActionLoggedEventArgs"/>.</param>
        public ActionLoggedEventArgs(DateTime timestamp, string message)
        {
            this.Timestamp = timestamp;
            this.Message = message;
        }

        /// <summary>
        /// Gets the timestamp of this <see cref="ActionLoggedEventArgs"/>.
        /// </summary>
        /// <value>The timestamp of this <see cref="ActionLoggedEventArgs"/>.</value>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets the message of the <see cref="ActionLoggedEventArgs"/>.
        /// </summary>
        /// <value>The message of the <see cref="ActionLoggedEventArgs"/>.</value>
        public string Message
        {
            get
            {
                return this.message;
            }

            private set
            {
                if (value.Length == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified message cannot be empty.");
                }

                this.message = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
