//-----------------------------------------------------------
// <copyright file="Program.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Program class.</summary>
//-----------------------------------------------------------
namespace ChatRoom.Model.ConsoleModel
{
    using ChatRoom.Model.Applications;

    /// <summary>
    /// Represents the <see cref="Program"/> class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// This is the entry point of the application.
        /// </summary>
        /// <param name="args">Possibly specified command line arguments.</param>
        private static void Main(string[] args)
        {
            Application application = new Application();
            application.Run();
        }
    }
}
