using DiscordModNotifiyer.Apis;
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
        /// Global name of the Settings file
        /// </summary>
        private const string SETTINGS_FILENAME = "Settings.json";

        /// <summary>
        /// Steam Api Link for a list of all games
        /// </summary>
        public const string STEAM_API_GAME_LIST_URL = "https://api.steampowered.com/ISteamApps/GetAppList/v2/";

        /// <summary>
        /// Steam Api Link for a Collection id
        /// </summary>
        public const string STEAM_API_COLLECTION_URL = "https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/?format=json";

        /// <summary>
        /// Steam Api Link for a Mod id
        /// </summary>
        public const string STEAM_API_FILE_DETAILS_URL = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/?format=json";

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

            steamApi.OnUpdatedModsFound += (sender, e) => _ = DiscordExtensions.SendHook(e.Mods);

            ConsoleExtensions.ClearConsole();

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);

                switch (cki.KeyChar)
                {
                    case '1':
                        _ = steamApi.UpdateSteamMods();
                        break;
                    case '2':
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
