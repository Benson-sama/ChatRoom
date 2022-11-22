//-------------------------------------------------------------------
// <copyright file="ExitChatMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ExitChatMessage class.</summary>
//-------------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    /// <summary>
    /// Represents the <see cref="ExitChatMessage"/> class.
    /// </summary>
    public class ExitChatMessage : Message
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ExitChatMessage"/> class.
        /// </summary>
        /// <param name="text">The content of the <see cref="ExitChatMessage"/>.</param>
        public ExitChatMessage(string text) : base(text)
        {
        }
    }
}
