//---------------------------------------------------------------------------------
// <copyright file="ChangeColourMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ChangeColourMessageSerialiser class.</summary>
//---------------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="ChangeColourMessageSerialiser"/> class.
    /// Its leading byte identifier is 0100 0000.
    /// </summary>
    public static class ChangeColourMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="ChangeColourMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="ChangeColourMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(ChangeColourMessage message)
        {
            byte[] data = message.Content.Serialise();
            byte messageIdentifier = 64;

            return data.Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to a <see cref="ChangeColourMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="ChangeColourMessage"/>.</returns>
        public static ChangeColourMessage Deserialise(byte[] data)
        {
            if (data[0] != 64)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 64.");
            }

            data = data.Skip(1).ToArray();
            string content = data.DeserialiseNextSectionToString(out data);

            return new ChangeColourMessage(content);
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to a <see cref="ChangeColourMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>A value indicating whether the specified
        /// byte array can be deserialised or not.</returns>
        public static bool CanDeserialise(byte[] data)
        {
            if (data == null)
            {
                return false;
            }

            if (data[0] == 64)
            {
                return true;
            }

            return false;
        }
    }
}
