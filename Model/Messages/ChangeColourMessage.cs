//----------------------------------------------------------------------
// <copyright file="ChangeColourMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ChangeColourMessage class.</summary>
//----------------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    /// <summary>
    /// Represents the <see cref="ChangeColourMessage"/> class.
    /// </summary>
    public class ChangeColourMessage : Message
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ChangeColourMessage"/> class.
        /// </summary>
        /// <param name="text">The text content of this <see cref="ChangeColourMessage"/>.</param>
        public ChangeColourMessage(string text) : base(text)
        {
        }
    }
}
