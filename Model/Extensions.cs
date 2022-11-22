//--------------------------------------------------------------
// <copyright file="Extensions.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Extensions class.</summary>
//--------------------------------------------------------------
namespace ChatRoom.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents the <see cref="Extensions"/> class containing extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Limits the given string to a given length and optionally appends an indicator "..."
        /// in case that the string is longer than the given length.
        /// </summary>
        /// <param name="text">The text to be limited.</param>
        /// <param name="number">The maximum length of the text.</param>
        /// <param name="showLongerMessageIndicator">A value indicating whether the message is
        /// longer indicator is appended.</param>
        /// <returns>The limited text.</returns>
        public static string LimitToNumber(this string text, int number, bool showLongerMessageIndicator)
        {
            if (text.Length <= number)
            {
                return text;
            }
            else
            {
                string limitedText = new string(text.Take(number).ToArray());

                if (showLongerMessageIndicator)
                {
                    limitedText += "...";
                }

                return limitedText;
            }
        }

        /// <summary>
        /// Splits the byte array into chunks of the same size.
        /// </summary>
        /// <param name="data">The byte array to be divided in chunks.</param>
        /// <param name="size">The size of the chunks.</param>
        /// <returns>An IEnumerable containing the chunks, if any.</returns>
        public static IEnumerable<byte[]> SplitInChunksWithSize(this byte[] data, int size)
        {
            byte[] chunk = new byte[size];
            int chunkIndex = 0;

            for (int i = 0; i < data.Length; i++)
            {
                chunk[chunkIndex++] = data[i];

                if (chunkIndex > 7)
                {
                    yield return chunk;
                    chunk = new byte[size];
                    chunkIndex = 0;
                }
            }

            yield return chunk;
        }

        /// <summary>
        /// Serialises a string into a byte array and indicates its length with a four byte leading identifier.
        /// </summary>
        /// <param name="text">The text to be serialised.</param>
        /// <returns>The serialised byte array.</returns>
        public static byte[] Serialise(this string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] lengthOfData = BitConverter.GetBytes(data.Length);

            return lengthOfData.Concat(data).ToArray();
        }

        /// <summary>
        /// Deserialises the next section of the data to a string. Removes the deserialised
        /// section and provides the trimmed data.
        /// </summary>
        /// <param name="data">The data, where the next section gets deserialised.</param>
        /// <param name="trimmedData">The trimmed data, where the deserialised part got removed.</param>
        /// <returns>The deserialised string.</returns>
        public static string DeserialiseNextSectionToString(this byte[] data, out byte[] trimmedData)
        {
            if (!data.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(data), "The specified data array must not be empty.");
            }

            int length = BitConverter.ToInt32(data, 0);
            string text = Encoding.UTF8.GetString(data.Skip(4).Take(length).ToArray());

            trimmedData = data.Skip(length + 4).ToArray();

            return text;
        }

        /// <summary>
        /// Deserialises the next section of the data to a integer. Removes the deserialised
        /// section and provides the trimmed data.
        /// </summary>
        /// <param name="data">The data, where the next section gets deserialised.</param>
        /// <param name="trimmedData">The trimmed data, where the deserialised part got removed.</param>
        /// <returns>The deserialised integer.</returns>
        public static int DeserialiseNextSectionToInt(this byte[] data, out byte[] trimmedData)
        {
            if (!data.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(data), "The specified data array must not be empty.");
            }

            int number = BitConverter.ToInt32(data, 0);

            trimmedData = data.Skip(4).ToArray();

            return number;
        }
    }
}
