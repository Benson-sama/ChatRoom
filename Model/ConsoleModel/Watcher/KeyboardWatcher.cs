//-----------------------------------------------------------------------
// <copyright file="KeyboardWatcher.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the KeyboardWatcher class.</summary>
//-----------------------------------------------------------------------
namespace ChatRoom.Model.ConsoleModel.Watcher
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents the <see cref="KeyboardWatcher"/> class.
    /// </summary>
    public class KeyboardWatcher
    {
        /// <summary>
        /// Arguments for the thread that monitors for pressed keys.
        /// </summary>
        private readonly KeyboardWatcherThreadArguments threadArguments;

        /// <summary>
        /// The thread that monitors the pressed key.
        /// </summary>
        private Thread thread;

        /// <summary>
        /// Initialises a new instance of the <see cref="KeyboardWatcher"/> class.
        /// </summary>
        public KeyboardWatcher()
        {
            this.threadArguments = new KeyboardWatcherThreadArguments();
        }

        /// <summary>
        /// Is fired when a key is pressed.
        /// </summary>
        public event EventHandler<OnKeyPressedEventArgs> OnKeyPressed;
        
        /// <summary>
        /// Is fired when the watcher has started.
        /// </summary>
        public event EventHandler AfterStarted;

        /// <summary>
        /// Is fired when the watcher has stopped.
        /// </summary>
        public event EventHandler AfterStopped;

        /// <summary>
        /// Starts the watcher.
        /// </summary>
        public void Start()
        {
            if (this.thread != null && this.thread.IsAlive)
            {
                return;
            }

            this.threadArguments.Exit = false;
            this.thread = new Thread(this.Worker);
            this.thread.Start(this.threadArguments);
        }

        /// <summary>
        /// Stops the watcher.
        /// </summary>
        public void Stop()
        {
            if (this.thread == null || !this.thread.IsAlive)
            {
                return;
            }

            this.threadArguments.Exit = true;
        }

        /// <summary>
        /// Represents the worker thread that monitors pressed keys.
        /// </summary>
        /// <param name="data">The thread arguments.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if the specified instance for data is not of type <see cref="KeyboardWatcherThreadArguments"/>.
        /// </exception>
        private void Worker(object data)
        {
            if (!(data is KeyboardWatcherThreadArguments))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"The specified data must be an instance of the {nameof(KeyboardWatcherThreadArguments)} class.");
            }

            KeyboardWatcherThreadArguments args = (KeyboardWatcherThreadArguments)data;

            this.AfterStarted?.Invoke(this, EventArgs.Empty);

            while (!args.Exit)
            {
                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                ConsoleKeyInfo cki = Console.ReadKey(true);
                this.OnKeyPressed?.Invoke(this, new OnKeyPressedEventArgs(cki.Key, cki.Modifiers));
            }

            this.AfterStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
