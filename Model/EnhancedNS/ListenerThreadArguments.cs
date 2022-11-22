//---------------------------------------------------------------------------
// <copyright file="ListenerThreadArguments.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the ListenerThreadArguments class.</summary>
//---------------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedNS
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Represents the <see cref="ListenerThreadArguments"/> class.
    /// </summary>
    public class ListenerThreadArguments
    {
        /// <summary>
        /// The delay between polling actions in milliseconds.
        /// </summary>
        private int pollDelay;

        /// <summary>
        /// The size of the read buffer.
        /// </summary>
        private int readBufferSize;

        /// <summary>
        /// The network stream of the ListenerThreadArguments.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// Initialises a new instance of the <see cref="ListenerThreadArguments"/> class.
        /// </summary>
        /// <param name="networkStream">The network stream of the ListenerThreadArguments.</param>
        public ListenerThreadArguments(NetworkStream networkStream)
        {
            this.Stream = networkStream;
            this.PollDelay = 50;
            this.ReadBufferSize = 8;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the listener thread shall exit or not.
        /// </summary>
        /// <value>The value indicating whether the listener thread shall exit or not.</value>
        public bool Exit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the polling delay of the listener thread.
        /// </summary>
        /// <value>The polling delay of the listener thread.</value>
        public int PollDelay
        {
            get
            {
                return this.pollDelay;
            }

            set
            {
                if (value < 10)
                {
                    throw new ArgumentOutOfRangeException(
                        "The specified value must be greater than 10 milliseconds.");
                }

                this.pollDelay = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the read buffer of the listener thread.
        /// </summary>
        /// <value>The size of the read buffer of the listener thread.</value>
        public int ReadBufferSize
        {
            get
            {
                return this.readBufferSize;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("The specified value must be greater than zero.");
                }

                this.readBufferSize = value;
            }
        }

        /// <summary>
        /// Gets the network stream of the listener thread.
        /// </summary>
        /// <value>The network stream of the listener thread.</value>
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
    }
}
