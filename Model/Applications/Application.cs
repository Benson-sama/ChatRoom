//---------------------------------------------------------------
// <copyright file="Application.cs" company="FH Wiener Neustadt">
//     Copyright (c) FH Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Benjamin Bogner</author>
// <summary>Contains the Application class.</summary>
//---------------------------------------------------------------
namespace ChatRoom.Model.Applications
{
    using System;
    using ChatRoom.Model.ApplicationMenu;
    using ChatRoom.Model.ConsoleModel;
    using ChatRoom.View;

    /// <summary>
    /// Represents the <see cref="Application"/> class.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// The renderer of the application, used to visualise many things in the console window.
        /// </summary>
        private readonly ChatRoomRenderer renderer;

        /// <summary>
        /// The main menu of the application.
        /// </summary>
        private readonly Menu mainMenu;

        /// <summary>
        /// The console settings of the Application class.
        /// </summary>
        private readonly ConsoleSettings consoleSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="Application"/> class.
        /// </summary>
        public Application()
        {
            this.renderer = new ChatRoomRenderer();
            this.mainMenu = new Menu();
            this.consoleSettings = new ConsoleSettings();
        }

        /// <summary>
        /// Starts the actual chat application.
        /// </summary>
        public void Run()
        {
            Console.Title = "Chat Room ©Benjamin BOGNER, 2022";
            Console.CursorVisible = false;

            while (true)
            {
                this.renderer.Draw(this.mainMenu);
                ConsoleKeyInfo cki = Console.ReadKey(true);
                switch (cki.Key)
                {
                    case ConsoleKey.UpArrow:
                        this.mainMenu.NavigateUp();
                        break;
                    case ConsoleKey.DownArrow:
                        this.mainMenu.NavigateDown();
                        break;
                    case ConsoleKey.Enter:
                        this.ExecuteMainMenuSelection();
                        break;
                }
            }
        }

        /// <summary>
        /// Starts the application as a server or client, depending on the selection.
        /// </summary>
        private void ExecuteMainMenuSelection()
        {
            switch (this.mainMenu.CurrentIndex)
            {
                case 0:
                    this.ExecuteStartServerCommand();
                    break;
                case 1:
                    this.ExecuteStartClientCommand();
                    break;
                case 2:
                    this.ExecuteExitApplicationCommand();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.mainMenu.CurrentIndex), "Menu index must be within limits.");
            }
        }

        /// <summary>
        /// Starts the server of the application.
        /// </summary>
        private void ExecuteStartServerCommand()
        {
            ServerApplication serverApplication = new ServerApplication();
            serverApplication.ActionLogged += this.renderer.ServerApplication_ActionLogged;
            serverApplication.Run();
        }

        /// <summary>
        /// Starts the chat client of the application.
        /// </summary>
        private void ExecuteStartClientCommand()
        {
            ClientApplication clientApplication = new ClientApplication(this.renderer);
            clientApplication.Run();
        }

        /// <summary>
        /// Terminates the application and resets the consoles size, colours and cursor visibility.
        /// </summary>
        private void ExecuteExitApplicationCommand()
        {
            this.consoleSettings.Restore();
            Console.Clear();
            Environment.Exit(0);
        }
    }
}
