//----------------------------------------------------------------------
// <copyright file="UserListSerialiser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the UserListSerialiser class.</summary>
//----------------------------------------------------------------------
namespace ChatRoom.Model.Serialiser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the <see cref="UserListSerialiser"/> class.
    /// Its leading byte identifier is 0001 0000.
    /// </summary>
    public static class UserListSerialiser
    {
        /// <summary>
        /// Serialises a given list of users.
        /// </summary>
        /// <param name="users">The specified list of users.</param>
        /// <returns>The serialised array of bytes.</returns>
        public static byte[] Serialise(List<User> users)
        {
            if (!users.Any())
            {
                return new byte[0];
            }

            // Build the header of the byte array, containing the number of elements of the list.
            byte[] userCountIdentifier = BitConverter.GetBytes(users.Count);
            byte dataIdentifier = 16;
            byte[] data = userCountIdentifier.Prepend(dataIdentifier).ToArray();

            // Build each user and concat them to one array.
            for (int i = 0; i < users.Count; i++)
            {
                byte[] endPointData = users[i].EndPoint.Serialise();
                byte[] idData = BitConverter.GetBytes(users[i].Id);
                byte[] nicknameData = users[i].Nickname.Serialise();
                byte[] colourData = users[i].MessageColour.Serialise();

                data = data.Concat(endPointData).Concat(idData)
                           .Concat(nicknameData).Concat(colourData).ToArray();
            }

            return data;
        }

        /// <summary>
        /// Deserialises a given byte array to a list of users.
        /// </summary>
        /// <param name="data">The specified byte array.</param>
        /// <returns>The deserialised list of users.</returns>
        public static List<User> Deserialise(byte[] data)
        {
            if (data[0] != 16)
            {
                throw new ArgumentOutOfRangeException(
                          nameof(data),
                          "The leading byte identifier of the specified array must be 16.");
            }

            if (!data.Any())
            {
                return new List<User>();
            }

            data = data.Skip(1).ToArray();
            int userCount = data.DeserialiseNextSectionToInt(out data);

            List<User> users = new List<User>();

            for (int i = 0; i < userCount; i++)
            {
                string endPoint = data.DeserialiseNextSectionToString(out data);
                int id = data.DeserialiseNextSectionToInt(out data);
                string nickname = data.DeserialiseNextSectionToString(out data);
                string messageColour = data.DeserialiseNextSectionToString(out data);

                users.Add(new User(id, nickname, messageColour, endPoint));
            }

            return users;
        }

        /// <summary>
        /// Checks whether the specified byte array can be
        /// deserialised to a list of users.
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

            if (data[0] == 16)
            {
                return true;
            }

            return false;
        }
    }
}
