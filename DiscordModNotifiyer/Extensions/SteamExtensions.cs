using DiscordModNotifiyer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordModNotifiyer.Extensions
{
    public class SteamExtensions
    {
        /// <summary>
        /// Api call to get steam player informations. There is a limit of steam ids by 100
        /// </summary>
        /// <param name="steamids">List of steamids where the information is needed</param>
        /// <returns>List of steam player informations</returns>
        public static async Task<List<SteamPlayerPlayerModel>> GetSteamPlayers(List<string> steamids)
        {
            try
            {
                var request = WebRequest.Create(Program.STEAM_API_PLAYER_URL + Program.Settings.SteamApiKey + "&steamids=" + String.Join(",", steamids));
                request.Method = "GET";

                using var webResponse = request.GetResponse();
                using var webStream = webResponse.GetResponseStream();

                using var reader = new StreamReader(webStream);
                var data = await reader.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<SteamPlayerModel>(data);

                return model.response.players;
            }
            catch (Exception e)
            {
                ConsoleExtensions.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get the gamename by a given appid
        /// </summary>
        /// <param name="appId">Appid of the game</param>
        /// <returns>Name of the game</returns>
        public static async Task<string> GetGameInfo(double appId)
        {
            var filename = "./gamelist.json";
            var reloadList = !File.Exists(filename) || File.GetLastWriteTime(filename).AddDays(1) < DateTime.Now;

            if (reloadList)
            {
                try
                {
                    var request = WebRequest.Create(Program.STEAM_API_GAME_LIST_URL);
                    request.Method = "GET";

                    using var webResponse = request.GetResponse();
                    using var webStream = webResponse.GetResponseStream();

                    using var reader = new StreamReader(webStream);
                    var data = await reader.ReadToEndAsync();

                    using (StreamWriter sw = File.CreateText(filename))
                    {
                        sw.WriteLine(data);
                    }
                }
                catch (Exception e)
                {
                    ConsoleExtensions.Error(e.Message);
                }
            }

            try
            {
                var content = File.ReadAllText(filename);
                var apps = JsonConvert.DeserializeObject<GameInfo>(content).applist.apps;
                return apps.FirstOrDefault(x => x.appid.Equals(appId)).name ?? "Unknown Gamename";
            }
            catch (Exception e)
            {
                ConsoleExtensions.Error(e.Message);
                return "Unknown Gamename";
            }
        }

        /// <summary>
        /// Get the information above a steam collection by a given id
        /// </summary>
        /// <param name="steamCollectionId">Id of the steam collection</param>
        /// <returns>Details of the collection by given collection id</returns>
        public static async Task<SteamFileDetailJsonDetailModel> GetCollectionInfo(double steamCollectionId)
        {
            var model = await GetPublishedFileDetails(new List<double> { steamCollectionId });
            return model?.response?.publishedfiledetails?.FirstOrDefault();
        }

        /// <summary>
        /// Method to get details above a given file. This file could be a steam collection id or a steam mod id.
        /// </summary>
        /// <typeparam name="T">Type of the json deserilize object that is needed</typeparam>
        /// <param name="files">List of file ids</param>
        /// <returns>Specific information above the given file ids</returns>
        public static async Task<SteamFileDetailJsonModel> GetPublishedFileDetails(List<double> files)
        {
            var parameters = new List<KeyValuePair<string, string>>();
            int i = 0;
            foreach (var id in files)
            {
                parameters.Add(new KeyValuePair<string, string>($"publishedfileids[{i++}]", id.ToString()));
            }
            parameters.Add(new KeyValuePair<string, string>("itemcount", i.ToString()));

            var httpClient = new HttpClient();

            using (var content = new FormUrlEncodedContent(parameters))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_FILE_DETAILS_URL, content);
                    return JsonConvert.DeserializeObject<SteamFileDetailJsonModel>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    ConsoleExtensions.Error(e.Message);
                    return null;
                }
            }
        }
    }
}
