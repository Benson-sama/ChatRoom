//--------------------------------------------------------------------------
// <copyright file="AliveMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the AliveMessageSerialiser class.</summary>
//--------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using System.Text;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="AliveMessageSerialiser"/> class.
    /// Its leading byte identifier is 0000 0001.
    /// </summary>
    public static class AliveMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="AliveMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="AliveMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(AliveMessage message)
        {
            byte[] data = message.Content.Serialise();
            byte messageIdentifier = 1;

            return data.Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to an <see cref="AliveMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="AliveMessage"/>.</returns>
        public static AliveMessage Deserialise(byte[] data)
        {
            if (data[0] != 1)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 1.");
            }

            // Skip the first two bytes as they are only identifiers and take as many values as stated in the second value.
            byte[] usageData = data.Skip(2).Take(Convert.ToInt32(data[1])).ToArray();

            return new AliveMessage(Encoding.UTF8.GetString(usageData));
        }

        /// <summary>
        /// Checks whether the specified byte array can be deserialised
        /// to an <see cref="AliveMessage"/>.
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

            if (data[0] == 1)
            {
                return true;
            }

            return false;
        }
    }
}
