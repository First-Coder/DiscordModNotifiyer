using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DiscordModNotifiyer.Extensions
{
    class ConsoleExtensions
    {
        /// <summary>
        /// Set default welcome messages
        /// </summary>
        public static void ClearConsole()
        {
            Console.Clear();

            Console.WriteLine(Environment.NewLine);
            WriteColor(@"[$$$$$$$$\ $$\                       $$\            $$$$$$\                  $$\]", ConsoleColor.DarkGreen);
            WriteColor(@"[$$  _____|\__|                      $$ |          $$  __$$\                 $$ |]", ConsoleColor.DarkGreen);
            WriteColor(@"[$$ |      $$\  $$$$$$\   $$$$$$$\ $$$$$$\         $$ /  \__| $$$$$$\   $$$$$$$ | $$$$$$\   $$$$$$\]", ConsoleColor.DarkGreen);
            WriteColor(@"[$$$$$\    $$ |$$  __$$\ $$  _____|\_$$  _|$$$$$$\ $$ |      $$  __$$\ $$  __$$ |$$  __$$\ $$  __$$\]", ConsoleColor.DarkGreen);
            WriteColor(@"[$$  __|   $$ |$$ |  \__|\$$$$$$\    $$ |  \______|$$ |      $$ /  $$ |$$ /  $$ |$$$$$$$$ |$$ |  \__|]", ConsoleColor.DarkGreen);
            WriteColor(@"[$$ |      $$ |$$ |       \____$$\   $$ |$$\       $$ |  $$\ $$ |  $$ |$$ |  $$ |$$   ____|$$ |]", ConsoleColor.DarkGreen);
            WriteColor(@"[$$ |      $$ |$$ |      $$$$$$$  |  \$$$$  |      \$$$$$$  |\$$$$$$  |\$$$$$$$ |\$$$$$$$\ $$ |]", ConsoleColor.DarkGreen);
            WriteColor(@"[\__|      \__|\__|      \_______/    \____/        \______/  \______/  \_______| \_______|\__|]", ConsoleColor.DarkGreen);
            Console.WriteLine(Environment.NewLine);

            WriteColor(@"[//--Informationen------------------------------------------------]", ConsoleColor.DarkGreen);
            WriteColor($"[// Title:] {Assembly.GetEntryAssembly().GetName().Name}", ConsoleColor.DarkGreen);
            WriteColor($"[// Version:] {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version}", ConsoleColor.DarkGreen);
            WriteColor($"[// Autor:] {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright}", ConsoleColor.DarkGreen);
            WriteColor(@"[//--Exit Codes---------------------------------------------------]", ConsoleColor.DarkGreen);
            WriteColor($"[// 0:] Application successful exited", ConsoleColor.DarkGreen);
            WriteColor($"[// 1:] Missing Steam API Key in the Settings.json", ConsoleColor.DarkGreen);
            WriteColor($"[// 2:] Missing Discord Web Hook in the Settings.json", ConsoleColor.DarkGreen);
            WriteColor(@"[//--Settings-----------------------------------------------------]", ConsoleColor.DarkGreen);
            WriteColor($"[// Automatic Refresh / Check:] {Program.Settings.AutomaticRefresh}", ConsoleColor.DarkGreen);
            WriteColor($"[// Automatic Refresh every (min):] {Program.Settings.AutomaticRefreshMin}", ConsoleColor.DarkGreen);
            WriteColor($"[// Check the Collection Id:] {Program.Settings.SteamCollection}", ConsoleColor.DarkGreen);
            var ids = Program.Settings.SteamCollection ? Program.Settings.SteamCollectionId.ToString() : String.Join(", ", Program.Settings.SteamModIds.ToArray());
            WriteColor($"[// Collection Id or Mod Ids:] {ids}", ConsoleColor.DarkGreen);
            WriteColor(@"[//--Options------------------------------------------------------]", ConsoleColor.DarkGreen);
            WriteColor($"[// 1:] Execute Refresh", ConsoleColor.DarkGreen);
            WriteColor($"[// 2:] Reload settings.json", ConsoleColor.DarkGreen);
            WriteColor($"[// ESC:] Close application", ConsoleColor.DarkGreen);
            WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.DarkGreen);
            Console.WriteLine(Environment.NewLine);

            if (string.IsNullOrEmpty(Program.Settings.SteamApiKey))
            {
                WriteColor(@"[//--No Steam API Key---------------------------------------------]", ConsoleColor.DarkRed);
                WriteColor($"[//:] Please insert a Steam API Key into the Settings.json", ConsoleColor.DarkRed);
                WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.DarkRed);
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(Program.Settings.SteamApiKey))
            {
                WriteColor(@"[//--No Discord Web Hook------------------------------------------]", ConsoleColor.DarkRed);
                WriteColor($"[//:] Please insert a Discord Web Hook into the Settings.json", ConsoleColor.DarkRed);
                WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.DarkRed);
                Environment.Exit(2);
            }
        }


        /// <summary>
        /// Write some coloring console messages for the user
        /// https://stackoverflow.com/questions/2743260/is-it-possible-to-write-to-the-console-in-colour-in-net
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="color">ConsoleColor value of the color</param>
        public static void WriteColor(string message, ConsoleColor color)
        {
            var pieces = Regex.Split(message, @"(\[[^\]]*\])");

            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                Console.Write(piece);
                Console.ResetColor();
            }

            Console.WriteLine();
        }
    }
}
