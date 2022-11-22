//---------------------------------------------------------------------
// <copyright file="EnhancedTcpClient.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the EnhancedTcpClient class.</summary>
//---------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedTCPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using ChatRoom.Model.EnhancedNS;
    using ChatRoom.Model.Messages;
    using ChatRoom.Model.Serialiser;

    /// <summary>
    /// Represents the <see cref="EnhancedTcpClient"/> class.
    /// </summary>
    public class EnhancedTcpClient
    {
        /// <summary>
        /// The <see cref="AliveThreadArguments"/> of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        private readonly AliveThreadArguments aliveThreadArguments;

        /// <summary>
        /// The <see cref="TcpClient"/> of the <see cref="EnhancedTcpClient"/>.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// The receive buffer of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        private List<byte> receiveBuffer;

        /// <summary>
        /// The alive polling interval in milliseconds.
        /// </summary>
        private int aliveInterval;

        /// <summary>
        /// The <see cref="EnhancedNS.EnhancedNetworkStream"/> of the <see cref="EnhancedTcpClient"/>.
        /// </summary>
        private EnhancedNetworkStream enhancedNetworkStream;

        /// <summary>
        /// The Alive <see cref="Thread"/> of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        private Thread aliveThread;

        /// <summary>
        /// Initialises a new instance of the <see cref="EnhancedTcpClient"/> class.
        /// </summary>
        /// <param name="tcpClient">The TCP-Client instance of the EnhancedTCPClient.</param>
        public EnhancedTcpClient(TcpClient tcpClient)
        {
            this.Client = tcpClient;
            this.EnhancedNetworkStream = new EnhancedNetworkStream(tcpClient.GetStream());
            this.EnhancedNetworkStream.DataReceived += this.OnDataReceived;
            this.AliveInterval = 50;
            this.aliveThreadArguments = new AliveThreadArguments();
            this.receiveBuffer = new List<byte>();
        }

        /// <summary>
        /// Is fired when the connection is established.
        /// </summary>
        public event EventHandler<ConnectedEventArgs> Connected;

        /// <summary>
        /// Is fired when the connection is closed.
        /// </summary>
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        /// <summary>
        /// Is fired when an <see cref="AliveMessage"/> is received.
        /// </summary>
        public event EventHandler<AliveMessageReceivedEventArgs> AliveMessageReceived;

        /// <summary>
        /// Is fired when data is received.
        /// </summary>
        public event EventHandler<EnhancedTcpClientDataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the <see cref="TcpClient"/> of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        /// <value>The <see cref="TcpClient"/> of the <see cref="EnhancedTcpClient"/>.</value>
        public TcpClient Client
        {
            get
            {
                return this.client;
            }

            private set
            {
                this.client = value ?? throw new ArgumentNullException(nameof(value), "The value cannot be null.");
            }
        }

        /// <summary>
        /// Gets or sets the alive polling interval time in milliseconds.
        /// </summary>
        /// <value>The alive polling interval time in milliseconds.</value>
        public int AliveInterval
        {
            get
            {
                return this.aliveInterval;
            }

            set
            {
                // Set a reasonable limit.
                if (value < 50)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The specified value cannot be less than 50.");
                }

                this.aliveInterval = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="EnhancedNS.EnhancedNetworkStream"/> of the <see cref="EnhancedTcpClient"/>.
        /// </summary>
        /// <value>The <see cref="EnhancedNS.EnhancedNetworkStream"/> of the <see cref="EnhancedTcpClient"/>.</value>
        public EnhancedNetworkStream EnhancedNetworkStream
        {
            get
            {
                return this.enhancedNetworkStream;
            }

            private set
            {
                this.enhancedNetworkStream = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }

        /// <summary>
        /// Starts the data monitoring of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        public void StartDataMonitoring()
        {
            this.EnhancedNetworkStream.StartListening();
        }

        /// <summary>
        /// Stops the data monitoring of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        public void StopDataMonitoring()
        {
            this.EnhancedNetworkStream.StopListening();
        }

        /// <summary>
        /// Starts the alive monitoring of this <see cref="EnhancedTcpClient"/>.
        /// </summary>
        public void StartAliveMonitoring()
        {
            if (this.aliveThread != null && this.aliveThread.IsAlive)
            {
                return;
            }

            this.aliveThreadArguments.Exit = false;
            this.aliveThread = new Thread(this.AliveWorker);
            this.aliveThread.Start(this.aliveThreadArguments);
        }

        /// <summary>
        /// Stops the alive monitoring of the <see cref="EnhancedTcpClient"/>.
        /// </summary>
        public void StopAliveMonitoring()
        {
            if (this.aliveThread == null || !this.aliveThread.IsAlive)
            {
                return;
            }

            this.aliveThreadArguments.Exit = true;
            this.aliveThread.Join();
        }

        /// <summary>
        /// Wraps the data in a custom protocol and writes it into the <see cref="ChatRoom.Model.EnhancedNS.EnhancedNetworkStream"/>.
        /// </summary>
        /// <param name="data">The specified data to be written.</param>
        public void Write(byte[] data)
        {
            var wrappedData = this.Wrap(data, Convert.ToByte(2));
            this.EnhancedNetworkStream.Write(wrappedData);
        }

        /// <summary>
        /// Checks which type of data is received and fires the corresponding event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="EnhancedNetworkStreamDataReceivedEventArgs"/> of the event.</param>
        private void OnDataReceived(object sender, EnhancedNetworkStreamDataReceivedEventArgs e)
        {
            this.receiveBuffer = this.receiveBuffer.Concat(e.Data).ToList();

            int currentPacketLength = BitConverter.ToInt32(this.receiveBuffer.ToArray(), 1);

            // If the next message is not completely arrived yet...
            if (this.receiveBuffer.Count <= currentPacketLength + 5)
            {
                // ...wait for new data.
                return;
            }

            byte identifier = this.receiveBuffer.First();
            var unwrappedData = this.Unwrap(this.receiveBuffer.ToArray());
            this.receiveBuffer = this.receiveBuffer.Skip(unwrappedData.Length + 5).ToList();

            if (identifier == 1)
            {
                if (AliveMessageSerialiser.CanDeserialise(unwrappedData))
                {
                    this.AliveMessageReceived?.Invoke(this, new AliveMessageReceivedEventArgs());
                }
            }
            else if (identifier == 2)
            {
                this.DataReceived?.Invoke(this, new EnhancedTcpClientDataReceivedEventArgs(e.Timestamp, unwrappedData, this));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Unknown message received.");
            }
        }

        /// <summary>
        /// Represents the alive worker thread of this <see cref="EnhancedTcpClient"/> that monitors the current connection status.
        /// </summary>
        /// <param name="data">The thread arguments.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if the specified instance for data is not of type <see cref="AliveThreadArguments"/>.
        /// </exception>
        private void AliveWorker(object data)
        {
            if (!(data is AliveThreadArguments))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"The specified data must be an instance of the {nameof(AliveThreadArguments)} class.");
            }

            AliveThreadArguments args = (AliveThreadArguments)data;

            if (this.Client.Connected)
            {
                this.Connected?.Invoke(this, new ConnectedEventArgs());
            }

            while (!args.Exit)
            {
                try
                {
                    AliveMessage aliveMessage = new AliveMessage("4L1V3");
                    var messageData = AliveMessageSerialiser.Serialise(aliveMessage);
                    var wrappedData = this.Wrap(messageData, Convert.ToByte(1));
                    this.EnhancedNetworkStream.Write(wrappedData);
                    Thread.Sleep(this.AliveInterval);
                }
                catch (Exception)
                {
                    this.Disconnected?.Invoke(this, new DisconnectedEventArgs());
                    args.Exit = true;
                }
            }
        }

        /// <summary>
        /// Wraps the specified data in a custom protocol.
        /// The identifier comes first, then the 4 byte length information,
        /// followed by the actual data.
        /// </summary>
        /// <param name="data">The specified data to be wrapped.</param>
        /// <param name="identifier">The given identifier to put on the first place.</param>
        /// <returns>The wrapped data.</returns>
        private byte[] Wrap(byte[] data, byte identifier)
        {
            var length = BitConverter.GetBytes(data.Length);
            return length.Concat(data).Prepend(identifier).ToArray();
        }

        /// <summary>
        /// Unwraps the given data, ignoring the leading byte identifier
        /// and retrieving as much data as stated in the 4 byte length information.
        /// </summary>
        /// <param name="data">The specified wrapped data.</param>
        /// <returns>The unwrapped data.</returns>
        private byte[] Unwrap(byte[] data)
        {
            int length = BitConverter.ToInt32(data, 1);
            return data.Skip(5).Take(length).ToArray();
        }
    }
}
