using DiscordModNotifiyer.Extensions;
using DiscordModNotifiyer.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DiscordModNotifiyer
{
    class Program
    {
        /// <summary>
        /// Global name of the settings file
        /// </summary>
        private const string SETTINGS_FILENAME = "settings.json";

        /// <summary>
        /// Settings json file
        /// </summary>
        public static Settings settings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILENAME));
            ConsoleExtensions.ClearConsole();

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);

                switch (cki.KeyChar)
                {
                    case '1':
                        
                        break;
                        //case '2':
                        //    Clear();
                        //    EnableProxy();
                        //    break;
                        //case '3':
                        //    Clear();
                        //    DisableProxy();
                        //    break;
                }
            } while (cki.Key != ConsoleKey.Escape);

            Environment.Exit(0);
        }

        /// <summary>
        /// Reload settings.json file and save them into the settings object
        /// </summary>
        public static void ReloadSettings() => settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILENAME));
    }
}
