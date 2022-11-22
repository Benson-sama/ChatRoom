//---------------------------------------------------------------------------
// <copyright file="ClientMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ClientMessageSerialiser class.</summary>
//---------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="ClientMessageSerialiser"/> class.
    /// Its leading byte identifier is 0000 0010.
    /// </summary>
    public static class ClientMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="ClientMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="ClientMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(ClientMessage message)
        {
            // Build all parts.
            byte[] colorData = message.Colour.Serialise();
            byte[] contentData = message.Content.Serialise();

            // Build entire message based on previous parts and return it.
            byte messageIdentifier = 2;
            return colorData.Concat(contentData).Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to a <see cref="ClientMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="ClientMessage"/>.</returns>
        public static ClientMessage Deserialise(byte[] data)
        {
            if (data[0] != 2)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 2.");
            }

            data = data.Skip(1).ToArray();
            string colour = data.DeserialiseNextSectionToString(out data);
            string content = data.DeserialiseNextSectionToString(out data);

            return new ClientMessage(colour, content);
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to a <see cref="ClientMessage"/>.
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

            if (data[0] == 2)
            {
                return true;
            }

            return false;
        }
    }
}
