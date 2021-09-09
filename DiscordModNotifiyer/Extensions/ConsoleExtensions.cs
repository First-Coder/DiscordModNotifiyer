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

            //if (string.IsNullOrEmpty(settings.ProxyIp))
            //{
            //    WriteColor(@"[//--Warning------------------------------------------------------]", ConsoleColor.Yellow);
            //    WriteColor($"[// ProxyIp:] Value is not set", ConsoleColor.Yellow);
            //    WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.Yellow);
            //}

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
            //WriteColor(@"[//--Exit Codes---------------------------------------------------]", ConsoleColor.DarkGreen);
            //WriteColor($"[// 0:] Application successful exited", ConsoleColor.DarkGreen);
            //WriteColor($"[// 1:] Supported OS is not given", ConsoleColor.DarkGreen);
            //WriteColor($"[// 2:] User has no root permissions", ConsoleColor.DarkGreen);
            //WriteColor($"[// 3:] Networksadapters are not set", ConsoleColor.DarkGreen);
            //WriteColor(@"[//--Settings-----------------------------------------------------]", ConsoleColor.DarkGreen);
            //WriteColor($"[// Call on Networkchange:] {settings.CallOnNetworkchange}", ConsoleColor.DarkGreen);
            //WriteColor($"[// Set proxy on Autostart:] {settings.SetProxyOnStartUp}", ConsoleColor.DarkGreen);
            //WriteColor($"[// Proxy status:] {status}", ConsoleColor.DarkGreen);
            //WriteColor(@"[//--Options------------------------------------------------------]", ConsoleColor.DarkGreen);
            //WriteColor($"[// 1:] Toggle \"Call on Networkchange\"", ConsoleColor.DarkGreen);
            //WriteColor($"[// 2:] Enable proxy", ConsoleColor.DarkGreen);
            //WriteColor($"[// 3:] Disable proxy", ConsoleColor.DarkGreen);
            //WriteColor($"[// ESC:] Close application", ConsoleColor.DarkGreen);
            WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.DarkGreen);
            Console.WriteLine(Environment.NewLine);

            //if (string.IsNullOrEmpty(settings.NetworkChangeAdapters))
            //{
            //    WriteColor(@"[//--No Networkadapters-------------------------------------------]", ConsoleColor.DarkRed);
            //    WriteColor($"[//:] Please insert Networkadapters (\"NetworkChangeAdapters\") in the settings.json", ConsoleColor.DarkRed);
            //    WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.DarkRed);
            //    if (!Debugger.IsAttached)
            //    {
            //        Environment.Exit(3);
            //    }
            //    else
            //    {
            //        Console.WriteLine(Environment.NewLine);
            //    }
            //}
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
