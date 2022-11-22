//-------------------------------------------------------------------------
// <copyright file="ChangeNicknameMessage.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ChangeNicknameMessage class.</summary>
//-------------------------------------------------------------------------
namespace ChatRoom.Model.Messages
{
    /// <summary>
    /// Represents the <see cref="ChangeNicknameMessage"/> class.
    /// </summary>
    public class ChangeNicknameMessage : Message
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ChangeNicknameMessage"/> class.
        /// </summary>
        /// <param name="text">The text content of this <see cref="ChangeNicknameMessage"/>.</param>
        public ChangeNicknameMessage(string text) : base(text)
        {
        }
    }
}
