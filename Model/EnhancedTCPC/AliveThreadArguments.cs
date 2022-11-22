//------------------------------------------------------------------------
// <copyright file="AliveThreadArguments.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the AliveThreadArguments class.</summary>
//------------------------------------------------------------------------
namespace ChatRoom.Model.EnhancedTCPC
{
    /// <summary>
    /// Represents the <see cref="AliveThreadArguments"/> class.
    /// </summary>
    public class AliveThreadArguments
    {
        /// <summary>
        /// Gets or sets a value indicating whether the alive thread of the <see cref="EnhancedTcpClient"/> shall exit.
        /// </summary>
        /// <value>The value indicating whether the alive thread of the <see cref="EnhancedTcpClient"/> shall exit.</value>
        public bool Exit
        {
            get;
            set;
        }
    }
}
