//---------------------------------------------------------------------------
// <copyright file="ServerMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ServerMessageSerialiser class.</summary>
//---------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="ServerMessageSerialiser"/> class.
    /// Its leading byte identifier is 0000 0100.
    /// </summary>
    public static class ServerMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="ServerMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="ServerMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(ServerMessage message)
        {
            byte[] timestampData = message.Timestamp.ToString().Serialise();
            byte[] senderData = message.Sender.Serialise();
            byte[] colourData = message.Colour.Serialise();
            byte[] contentData = message.Content.Serialise();
            byte messageIdentifier = 4;

            return timestampData.Concat(senderData).Concat(colourData).Concat(contentData).Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to a <see cref="ServerMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="ServerMessage"/>.</returns>
        public static ServerMessage Deserialise(byte[] data)
        {
            if (data[0] != 4)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 4.");
            }

            data = data.Skip(1).ToArray();
            DateTime timestamp = DateTime.Parse(data.DeserialiseNextSectionToString(out data));
            string sender = data.DeserialiseNextSectionToString(out data);
            string colour = data.DeserialiseNextSectionToString(out data);
            string content = data.DeserialiseNextSectionToString(out data);

            return new ServerMessage(timestamp, sender, colour, content);
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to a <see cref="ServerMessage"/>.
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

            if (data[0] == 4)
            {
                return true;
            }

            return false;
        }
    }
}
