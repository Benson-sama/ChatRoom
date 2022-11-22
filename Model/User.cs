//-----------------------------------------------------------
// <copyright file="User.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the User class.</summary>
//-----------------------------------------------------------
namespace ChatRoom.Model
{
    using System;

    /// <summary>
    /// Represents the <see cref="User"/> class.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The nickname of the <see cref="User"/>.
        /// </summary>
        private string nickname;

        /// <summary>
        /// The message colour of this <see cref="User"/>.
        /// </summary>
        private string messageColour;

        /// <summary>
        /// The EndPoint information of this <see cref="User"/>.
        /// </summary>
        private string endPoint;

        /// <summary>
        /// Initialises a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of this <see cref="User"/>.</param>
        /// <param name="nickname">The nickname of this <see cref="User"/>.</param>
        /// <param name="messageColour">The message colour of this <see cref="User"/>.</param>
        /// <param name="endPoint">The <see cref="System.Net.EndPoint"/> instance of this <see cref="User"/>.</param>
        public User(int id, string nickname, string messageColour, string endPoint)
        {
            this.Id = id;
            this.Nickname = nickname;
            this.MessageColour = messageColour;
            this.EndPoint = endPoint;
        }

        /// <summary>
        /// Gets or sets the unique identifier of this <see cref="User"/>.
        /// </summary>
        /// <value>The unique identifier of this <see cref="User"/>.</value>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the nickname of this <see cref="User"/>.
        /// </summary>
        /// <value>The nickname of this <see cref="User"/>.</value>
        public string Nickname
        {
            get
            {
                return this.nickname;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be empty.");
                }

                // Additional check in case of a nullable string.
                this.nickname = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null");
            }
        }

        /// <summary>
        /// Gets or sets the message colour of this <see cref="User"/>.
        /// </summary>
        /// <value>The message colour of this <see cref="User"/>.</value>
        public string MessageColour
        {
            get
            {
                return this.messageColour;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be empty.");
                }

                // Additional check in case of a nullable string.
                this.messageColour = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null");
            }
        }

        /// <summary>
        /// Gets the EndPoint information of this <see cref="User"/>.
        /// </summary>
        /// <value>The EndPoint information of this <see cref="User"/>.</value>
        public string EndPoint
        {
            get
            {
                return this.endPoint;
            }

            private set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be empty.");
                }

                // Additional check in case of a nullable string.
                this.endPoint = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
