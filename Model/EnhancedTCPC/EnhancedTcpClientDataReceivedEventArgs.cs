//------------------------------------------------------------------------------------------
// <copyright file="EnhancedTcpClientDataReceivedEventArgs.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the EnhancedTcpClientDataReceivedEventArgs class.</summary>
//------------------------------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedTCPC
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents the <see cref="EnhancedTcpClientDataReceivedEventArgs"/> class.
    /// </summary>
    public class EnhancedTcpClientDataReceivedEventArgs
    {
        /// <summary>
        /// The data of the <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.
        /// </summary>
        private EnhancedTcpClient enhancedTcpClient;

        /// <summary>
        /// Initialises a new instance of the
        /// <see cref="EnhancedTcpClientDataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp of this event.</param>
        /// <param name="data">The data of this event.</param>
        /// <param name="enhancedTcpClient">The <see cref="EnhancedTCPC.EnhancedTcpClient"/> of the event.</param>
        public EnhancedTcpClientDataReceivedEventArgs(DateTime timestamp, byte[] data, EnhancedTcpClient enhancedTcpClient)
        {
            this.Timestamp = timestamp;
            this.Data = data;
            this.EnhancedTcpClient = enhancedTcpClient;
        }

        /// <summary>
        /// Gets the <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.
        /// </summary>
        /// <value>The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.</value>
        public EnhancedTcpClient EnhancedTcpClient
        {
            get
            {
                return this.enhancedTcpClient;
            }

            private set
            {
                this.enhancedTcpClient = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null");
            }
        }

        /// <summary>
        /// Gets the timestamp of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.
        /// </summary>
        /// <value>The timestamp of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.</value>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets a copy of the data byte array of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.
        /// </summary>
        /// <value>The copy of the data byte array of this <see cref="EnhancedTcpClientDataReceivedEventArgs"/>.</value>
        public byte[] Data
        {
            get
            {
                return this.data.ToArray();
            }

            private set
            {
                this.data = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }
    }
}
