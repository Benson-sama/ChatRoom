//-------------------------------------------------------------------------------
// <copyright file="UserUpdateMessageSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the UserUpdateMessageSerialiser class.</summary>
//-------------------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Linq;
    using ChatRoom.Model.Messages;

    /// <summary>
    /// Represents the <see cref="UserUpdateMessageSerialiser"/> class.
    /// Its leading byte identifier is 0000 1000.
    /// </summary>
    public static class UserUpdateMessageSerialiser
    {
        /// <summary>
        /// Serialises a given <see cref="UserUpdateMessage"/>.
        /// </summary>
        /// <param name="message">The specified <see cref="UserUpdateMessage"/>.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(UserUpdateMessage message)
        {
            byte[] contentData = message.Content.Serialise();
            byte[] endPointData = message.User.EndPoint.Serialise();
            byte[] idData = BitConverter.GetBytes(message.User.Id);
            byte[] nicknameData = message.User.Nickname.Serialise();
            byte[] colourData = message.User.MessageColour.Serialise();
            byte messageIdentifier = 8;

            return contentData.Concat(endPointData).Concat(idData)
                              .Concat(nicknameData).Concat(colourData)
                              .Prepend(messageIdentifier).ToArray();
        }

        /// <summary>
        /// Deserialises a given byte array to an <see cref="UserUpdateMessage"/>.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised <see cref="UserUpdateMessage"/>.</returns>
        public static UserUpdateMessage Deserialise(byte[] data)
        {
            if (data[0] != 8)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 8.");
            }

            data = data.Skip(1).ToArray();

            string content = data.DeserialiseNextSectionToString(out data);
            string endPoint = data.DeserialiseNextSectionToString(out data);
            int id = data.DeserialiseNextSectionToInt(out data);
            string nickname = data.DeserialiseNextSectionToString(out data);
            string colour = data.DeserialiseNextSectionToString(out data);

            return new UserUpdateMessage(content, new User(id, nickname, colour, endPoint));
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to a <see cref="UserUpdateMessage"/>.
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

            if (data[0] == 8)
            {
                return true;
            }

            return false;
        }
    }
}
