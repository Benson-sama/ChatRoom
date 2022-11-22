//-----------------------------------------------------------------------------
// <copyright file="ExitChatMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ExitChatMessageSerialiser class.</summary>
//-----------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="ExitChatMessageSerialiser"/> class.
    /// Its leading byte identifier is 1000 0000.
    /// </summary>
    public static class ExitChatMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="ExitChatMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="ExitChatMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(ExitChatMessage message)
        {
            byte[] data = message.Content.Serialise();
            byte messageIdentifier = 128;

            return data.Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to an <see cref="ExitChatMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="ExitChatMessage"/>.</returns>
        public static ExitChatMessage Deserialise(byte[] data)
        {
            if (data[0] != 128)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 128.");
            }

            data = data.Skip(1).ToArray();
            string content = data.DeserialiseNextSectionToString(out data);

            return new ExitChatMessage(content);
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to an <see cref="ExitChatMessage"/>.
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

            if (data[0] == 128)
            {
                return true;
            }

            return false;
        }
    }
}
