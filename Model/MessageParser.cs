//-----------------------------------------------------------------
// <copyright file="MessageParser.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the MessageParser class.</summary>
//-----------------------------------------------------------------
namespace ChatRoom.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ChatRoom.Model.EnhancedTCPC;

    /// <summary>
    /// Represents the <see cref="MessageParser"/> class.
    /// </summary>
    public class MessageParser
    {
        /// <summary>
        /// The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="MessageParser"/>.
        /// </summary>
        private EnhancedTcpClient enhancedTcpClient;

        /// <summary>
        /// The receive data buffer for this <see cref="MessageParser"/>.
        /// </summary>
        private List<byte> receiveBuffer;

        /// <summary>
        /// The send buffer size of this <see cref="MessageParser"/>.
        /// </summary>
        private int sendBufferSize;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageParser"/> class.
        /// </summary>
        /// <param name="client">The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// used for collecting data
        /// and sending parsed data.</param>
        /// <param name="sendBufferSize">The send buffer size of this <see cref="MessageParser"/>.</param>
        public MessageParser(EnhancedTcpClient client, int sendBufferSize)
        {
            this.receiveBuffer = new List<byte>();
            this.EnhancedTcpClient = client;
            this.SendBufferSize = sendBufferSize;
        }

        /// <summary>
        /// The event that gets fired, when an invalid message gets received.
        /// Invalid messages result from differing checksums.
        /// </summary>
        public event EventHandler<InvalidMessageReceivedEventArgs> InvalidMessageReceived;

        /// <summary>
        /// The event that gets fired when a complete message data is parsed.
        /// </summary>
        public event EventHandler<MessageParsedEventArgs> MessageParsed;

        /// <summary>
        /// Gets the <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="MessageParser"/>.
        /// </summary>
        /// <value>The <see cref="ChatRoom.Model.EnhancedTCPC.EnhancedTcpClient"/>
        /// of this <see cref="MessageParser"/>.</value>
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
                                                            "The specified value cannot be null.");
            }
        }

        /// <summary>
        /// Gets the send buffer size of this <see cref="MessageParser"/>.
        /// </summary>
        /// <value>The send buffer size of this <see cref="MessageParser"/>.</value>
        public int SendBufferSize
        {
            get
            {
                return this.sendBufferSize;
            }

            private set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                              nameof(value),
                              "The specified value cannot be less than 1.");
                }

                if (value != 1 && (value % 2) != 0)
                {
                    string exceptionText = "The specified value must correspond to a multiple of 2. " +
                                           "(2 is also allowed)";

                    throw new ArgumentOutOfRangeException(nameof(value), exceptionText);
                }

                this.sendBufferSize = value;
            }
        }

        /// <summary>
        /// Collects data from the data received event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        public void Collect(object sender, EnhancedTcpClientDataReceivedEventArgs e)
        {
            this.Collect(e.Data);
        }

        /// <summary>
        /// Collects the specified data and adds it to the internal buffer, if it is valid.
        /// </summary>
        /// <param name="data">The specified data.</param>
        public void Collect(byte[] data)
        {
            if (!data.Any() ||
                (!this.receiveBuffer.Any() && data[0] != 69))
            {
                return;
            }

            // Either initialise or append.
            if (data[0] == 69)
            {
                this.receiveBuffer = data.Skip(1).ToList();
            }
            else
            {
                this.receiveBuffer.AddRange(data);
            }

            // Fire the message parsed event if it is complete.
            int length = BitConverter.ToInt32(this.receiveBuffer.Take(4).ToArray(), 0);

            if (this.receiveBuffer.Count >= length + 5)
            {
                byte[] messageData = this.receiveBuffer.Skip(4).Take(length).ToArray();
                byte checksum = this.GetChecksum(messageData);
                if (checksum == this.receiveBuffer[length + 4])
                {
                    this.MessageParsed?.Invoke(
                                        this,
                                        new MessageParsedEventArgs(
                                            DateTime.Now,
                                            messageData,
                                            this.EnhancedTcpClient));
                }
                else
                {
                    this.InvalidMessageReceived?.Invoke(this, new InvalidMessageReceivedEventArgs());
                }

                this.receiveBuffer.Clear();
            }
        }

        /// <summary>
        /// Wraps the specified data in a unique message parsing protocol and sends it
        /// in send buffer sizes using the underlying <see cref="EnhancedTCPC.EnhancedTcpClient"/>.
        /// </summary>
        /// <param name="data">The data that gets parsed and sent.</param>
        public void ParseAndSend(byte[] data)
        {
            if (!data.Any())
            {
                return;
            }

            // Build protocol data.
            byte identifier = 69;
            var length = BitConverter.GetBytes(data.Length);
            byte checksum = this.GetChecksum(data);
            var wrappedData = length.Concat(data).Prepend(identifier).Append(checksum).ToArray();
            var chunkedWrappedData = wrappedData.SplitInChunksWithSize(this.SendBufferSize);

            try
            {
                foreach (var chunk in chunkedWrappedData)
                {
                    this.EnhancedTcpClient.Write(chunk);
                }
            }
            catch (Exception)
            {
                // Connection error.
            }
        }

        /// <summary>
        /// Calculates a checksum byte by adding every single value of the
        /// specified data.
        /// </summary>
        /// <param name="data">The specified data.</param>
        /// <returns>The checksum as a byte.</returns>
        private byte GetChecksum(byte[] data)
        {
            byte checksum = new byte();

            foreach (var value in data)
            {
                checksum += value;
            }

            return checksum;
        }
    }
}
