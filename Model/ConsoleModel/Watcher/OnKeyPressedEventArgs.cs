//-----------------------------------------------------------------------
// <copyright file="OnKeyPressedEventArgs.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the OnKeyPressedEventArgs class.</summary>
//-----------------------------------------------------------------------
namespace ChatRoom.Model.ConsoleModel.Watcher
{
    using System;

    /// <summary>
    /// Represents the <see cref="OnKeyPressedEventArgs"/> class.
    /// </summary>
    public class OnKeyPressedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="OnKeyPressedEventArgs"/> class.
        /// </summary>
        /// <param name="key">The key that has been pressed.</param>
        /// <param name="modifiers">The modifiers that have been pressed.</param>
        public OnKeyPressedEventArgs(ConsoleKey key, ConsoleModifiers modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the key that has been pressed.
        /// </summary>
        /// <value>The key that has been pressed.</value>
        public ConsoleKey Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the modifiers that have been pressed.
        /// </summary>
        /// <value>The modifiers that have been pressed.</value>
        public ConsoleModifiers Modifiers
        {
            get;
            private set;
        }
    }
}
