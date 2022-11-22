//-----------------------------------------------------------------
// <copyright file="ClientMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ClientMessage class.</summary>
//-----------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    using System;

    /// <summary>
    /// Represents the <see cref="ClientMessage"/> class.
    /// </summary>
    public class ClientMessage : Message
    {
        /// <summary>
        /// The colour of this <see cref="ClientMessage"/>.
        /// </summary>
        private string colour;

        /// <summary>
        /// Initialises a new instance of the <see cref="ClientMessage"/> class.
        /// </summary>
        /// <param name="colour">The colour of this <see cref="ClientMessage"/>.</param>
        /// <param name="text">The text content of this <see cref="ClientMessage"/>.</param>
        public ClientMessage(string colour, string text) : base(text)
        {
            this.Colour = colour;
        }

        /// <summary>
        /// Gets the colour of this <see cref="ClientMessage"/>.
        /// </summary>
        /// <value>The colour of this <see cref="ClientMessage"/>.</value>
        public string Colour
        {
            get
            {
                return this.colour;
            }

            private set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified color cannot be empty.");
                }

                this.colour = value ?? throw new ArgumentNullException(nameof(value), "The specified color cannot be null.");
            }
        }
    }
}
