//---------------------------------------------------------------------
// <copyright file="UserUpdateMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the UserUpdateMessage class.</summary>
//---------------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    using System;

    /// <summary>
    /// Represents the <see cref="UserUpdateMessage"/> that is used for
    /// informing clients about changes regarding a specific <see cref="ChatRoom.User"/>.
    /// </summary>
    public class UserUpdateMessage : Message
    {
        /// <summary>
        /// The <see cref="ChatRoom.User"/> that is affected by a change.
        /// </summary>
        private User user;

        /// <summary>
        /// Initialises a new instance of the <see cref="UserUpdateMessage"/> class.
        /// </summary>
        /// <param name="changesText">The change that the <see cref="ChatRoom.User"/> is affected of.</param>
        /// <param name="user">The <see cref="ChatRoom.User"/> that is affected of the change.</param>
        public UserUpdateMessage(string changesText, User user) : base(changesText)
        {
            this.User = user;
        }

        /// <summary>
        /// Gets the <see cref="ChatRoom.User"/> of this <see cref="UserUpdateMessage"/>.
        /// </summary>
        /// <value>The <see cref="ChatRoom.User"/> of this <see cref="UserUpdateMessage"/>.</value>
        public User User
        {
            get
            {
                return this.user;
            }

            private set
            {
                this.user = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
