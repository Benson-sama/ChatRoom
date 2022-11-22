//----------------------------------------------------------------------------------------------
// <copyright file="EnhancedNetworkStreamDataReceivedEventArgs.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the EnhancedNetworkStreamDataReceivedEventArgs class.</summary>
//----------------------------------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedNS
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents the <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/> class.
    /// </summary>
    public class EnhancedNetworkStreamDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The data of the <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Initialises a new instance of the <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp of this <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.</param>
        /// <param name="data">The data byte array of this <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.</param>
        public EnhancedNetworkStreamDataReceivedEventArgs(DateTime timestamp, byte[] data)
        {
            this.Timestamp = timestamp;
            this.Data = data;
        }

        /// <summary>
        /// Gets the timestamp of this <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.
        /// </summary>
        /// <value>The timestamp of this <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.</value>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Gets a copy of the data byte array of this <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.
        /// </summary>
        /// <value>The copy of the data byte array of this <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/>.</value>
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
