//-------------------------------------------------------------------------
// <copyright file="EnhancedNetworkStream.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the EnhancedNetworkStream class.</summary>
//-------------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedNS
{
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Represents the <see cref="EnhancedNetworkStream"/> class.
    /// </summary>
    public class EnhancedNetworkStream
    {
        /// <summary>
        /// The listener thread arguments of the EnhancedNetworkStream.
        /// </summary>
        private readonly ListenerThreadArguments listenerThreadArguments;

        /// <summary>
        /// The locking object for write access of the underlying <see cref="NetworkStream"/>.
        /// </summary>
        private readonly object streamLocker;

        /// <summary>
        /// The listener thread of the EnhancedNetworkStream.
        /// </summary>
        private Thread listenerThread;

        /// <summary>
        /// The network stream of the EnhancedNetworkStream.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// Initialises a new instance of the <see cref="EnhancedNetworkStream"/> class.
        /// </summary>
        /// <param name="networkStream">The network stream of the EnhancedNetworkStream.</param>
        public EnhancedNetworkStream(NetworkStream networkStream)
        {
            this.streamLocker = new object();
            this.Stream = networkStream;
            this.listenerThreadArguments = new ListenerThreadArguments(this.Stream);
        }

        /// <summary>
        /// Is fired when data is received.
        /// </summary>
        public event EventHandler<EnhancedNetworkStreamDataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the network stream of the EnhancedNetworkStream.
        /// </summary>
        /// <value>The network stream of the EnhancedNetworkStream.</value>
        public NetworkStream Stream
        {
            get
            {
                return this.stream;
            }

            private set
            {
                this.stream = value ?? throw new ArgumentNullException(nameof(value), "The specified value cannot be null.");
            }
        }

        /// <summary>
        /// Starts the network stream listener.
        /// </summary>
        public void StartListening()
        {
            if (this.listenerThread != null && this.listenerThread.IsAlive)
            {
                return;
            }

            this.listenerThreadArguments.Exit = false;
            this.listenerThread = new Thread(this.ListenerWorker);
            this.listenerThread.Start(this.listenerThreadArguments);
        }

        /// <summary>
        /// Stops the network stream listener.
        /// </summary>
        public void StopListening()
        {
            if (this.listenerThread == null || !this.listenerThread.IsAlive)
            {
                return;
            }

            this.listenerThreadArguments.Exit = true;
        }

        /// <summary>
        /// Writes data to the network stream instance.
        /// </summary>
        /// <param name="data">The network stream instance.</param>
        public void Write(byte[] data)
        {
            lock (this.streamLocker)
            {
                if (!this.Stream.CanWrite)
                {
                    throw new Exception("Cannot write to the network stream.");
                }

                this.Stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Represents the worker thread that monitors arriving data.
        /// </summary>
        /// <param name="data">The thread arguments.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if the specified instance for data is not of type <see cref="ListenerThreadArguments"/>.
        /// </exception>
        private void ListenerWorker(object data)
        {
            if (!(data is ListenerThreadArguments))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"The specified data must be an instance of the {nameof(ListenerThreadArguments)} class.");
            }

            ListenerThreadArguments args = (ListenerThreadArguments)data;

            while (!args.Exit)
            {
                // Check whether data is available or not.
                if (!this.stream.DataAvailable)
                {
                    Thread.Sleep(args.PollDelay);
                    continue;
                }

                byte[] receiveBuffer = new byte[args.ReadBufferSize];
                int receivedBytes = this.stream.Read(receiveBuffer, 0, receiveBuffer.Length);

                this.DataReceived?.Invoke(
                    this,
                    new EnhancedNetworkStreamDataReceivedEventArgs(DateTime.Now, receiveBuffer.Take(receivedBytes).ToArray()));
            }
        }
    }
}
