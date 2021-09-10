using DiscordModNotifiyer.Apis;
using DiscordModNotifiyer.Extensions;
using DiscordModNotifiyer.Models;
using JNogueira.Discord.Webhook.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordModNotifiyer
{
    class Program
    {
        /// <summary>
        /// Global name of the Settings file
        /// </summary>
        private const string SETTINGS_FILENAME = "Settings.json";

        /// <summary>
        /// Steam Api Link for a Collection id
        /// </summary>
        public const string STEAM_API_COLLECTION_URL = "https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/?format=json";

        /// <summary>
        /// Steam Api Link for a Mod id
        /// </summary>
        public const string STEAM_API_MOD_URL = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/?format=json";

        /// <summary>
        /// Steam Api Link for a player information
        /// </summary>
        public const string STEAM_API_PLAYER_URL = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002?key=";

        /// <summary>
        /// Settings json file
        /// </summary>
        public static Settings Settings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILENAME));

            var steamApi = new SteamApi();
            var discordApi = new DiscordApi();

            steamApi.OnUpdatedModsFound += (sender, e) => _ = discordApi.SendHook(sender, e);

            ConsoleExtensions.ClearConsole();

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);

                switch (cki.KeyChar)
                {
                    case '1':
                        ConsoleExtensions.WriteColor(@"[//--Execute Refresh----------------------------------------------]", ConsoleColor.DarkGreen);
                        _ = steamApi.UpdateSteamMods();
                        break;
                    case '2':

                        break;
                    case '3':
                        ReloadSettings();
                        ConsoleExtensions.ClearConsole();
                        steamApi.RefreshSettings();
                        break;
                }
            } while (cki.Key != ConsoleKey.Escape);

            Environment.Exit(0);
        }

        /// <summary>
        /// Reload Settings.json file and save them into the Settings object
        /// </summary>
        public static void ReloadSettings() => Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILENAME));
    }
}
