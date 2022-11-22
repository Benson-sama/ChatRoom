//----------------------------------------------------------------
// <copyright file="AliveMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the AliveMessage class.</summary>
//----------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    /// <summary>
    /// Represents the <see cref="AliveMessage"/> class.
    /// </summary>
    public class AliveMessage : Message
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AliveMessage"/> class.
        /// </summary>
        /// <param name="text">The text content of this <see cref="AliveMessage"/>.</param>
        public AliveMessage(string text) : base(text)
        {
        }
    }
}
