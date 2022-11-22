//--------------------------------------------------------------------------
// <copyright file="MessageParsedEventArgs.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the MessageParsedEventArgs class.</summary>
//--------------------------------------------------------------------------
namespace ChatRoom.Model
{
    using System;
    using System.Linq;
    using ChatRoom.Model.EnhancedTCPC;

    /// <summary>
    /// Represents the <see cref="MessageParsedEventArgs"/> class.
    /// </summary>
    public class MessageParsedEventArgs
    {
        /// <summary>
        /// The data of this <see cref="MessageParsedEventArgs"/>.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="MessageParsedEventArgs"/>.
        /// </summary>
        private EnhancedTcpClient enhancedTcpClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageParsedEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp of the <see cref="MessageParsedEventArgs"/>.</param>
        /// <param name="data">The data of the <see cref="MessageParsedEventArgs"/>.</param>
        /// <param name="enhancedTcpClient">The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of the <see cref="MessageParsedEventArgs"/>.</param>
        public MessageParsedEventArgs(DateTime timestamp, byte[] data, EnhancedTcpClient enhancedTcpClient)
        {
            this.Timestamp = timestamp;
            this.Data = data;
            this.EnhancedTcpClient = enhancedTcpClient;
        }

        /// <summary>
        /// Gets the <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="MessageParsedEventArgs"/>.
        /// </summary>
        /// <value>The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="MessageParsedEventArgs"/>.</value>
        public EnhancedTcpClient EnhancedTcpClient
        {
            get
            {
                return this.enhancedTcpClient;
            }

            private set
            {
                this.enhancedTcpClient = value ?? throw new ArgumentNullException(
                                                            nameof(value),
                                                            "The specified value cannot be null");
            }
        }

        /// <summary>
        /// Gets the timestamp of this <see cref="MessageParsedEventArgs"/>.
        /// </summary>
        /// <value>The timestamp of this <see cref="MessageParsedEventArgs"/>.</value>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets a copy of the data byte array of this <see cref="MessageParsedEventArgs"/>.
        /// </summary>
        /// <value>The copy of the data byte array of this <see cref="MessageParsedEventArgs"/>.</value>
        public byte[] Data
        {
            get
            {
                return this.data.ToArray();
            }

            private set
            {
                this.data = value ?? throw new ArgumentNullException(
                                               nameof(value),
                                               "The specified value cannot be null.");
            }
        }
    }
}