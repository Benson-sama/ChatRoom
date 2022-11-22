//-----------------------------------------------------------------------------------
// <copyright file="ChangeNicknameMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ChangeNicknameMessageSerialiser class.</summary>
//-----------------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="ChangeNicknameMessageSerialiser"/> class.
    /// Its leading byte identifier is 0010 0000.
    /// </summary>
    public static class ChangeNicknameMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="ChangeNicknameMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="ChangeNicknameMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(ChangeNicknameMessage message)
        {
            byte[] data = message.Content.Serialise();
            byte messageIdentifier = 32;

            return data.Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to a <see cref="ChangeNicknameMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="ChangeNicknameMessage"/>.</returns>
        public static ChangeNicknameMessage Deserialise(byte[] data)
        {
            if (data[0] != 32)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 32.");
            }

            data = data.Skip(1).ToArray();
            string content = data.DeserialiseNextSectionToString(out data);

            return new ChangeNicknameMessage(content);
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to a <see cref="ChangeNicknameMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>A value indicating whether the specified byte array
        /// can be deserialised or not.</returns>
        public static bool CanDeserialise(byte[] data)
        {
            if (data == null)
            {
                return false;
            }

            if (data[0] == 32)
            {
                return true;
            }

            return false;
        }
    }
}
