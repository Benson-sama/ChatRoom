//---------------------------------------------------------------------
// <copyright file="DisconnectedEventArgs.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the DisconnectedEventArgs class.</summary>
//---------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedTCPC
{
    using System;

    /// <summary>
    /// Represents the <see cref="DisconnectedEventArgs"/> class.
    /// </summary>
    public class DisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DisconnectedEventArgs"/> class.
        /// </summary>
        public DisconnectedEventArgs()
        {
        }
    }
}
