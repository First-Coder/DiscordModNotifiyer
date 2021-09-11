using DiscordModNotifiyer.Events;
using DiscordModNotifiyer.Extensions;
using DiscordModNotifiyer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordModNotifiyer.Apis
{
    class SteamApi
    {
        public event EventHandler<UpdatedModsEventArgs> OnUpdatedModsFound;

        private System.Timers.Timer timer;

        #region Load / Refresh Timer
        public SteamApi() => RefreshSettings();
        public void RefreshSettings()
        {
            timer = new System.Timers.Timer(Program.Settings.AutomaticRefreshMin * 60 * 1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = Program.Settings.AutomaticRefresh;
        }
        #endregion

        #region Static methods
        public static async Task<List<SteamPlayerPlayerModel>> GetSteamPlayers(List<string> steamids)
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

        public static async Task<string> GetGameInfo(double appId)
        {
            var filename = "./gamelist.json";
            var reloadList = !File.Exists(filename) || File.GetLastWriteTime(filename).AddDays(1) < DateTime.Now;

            if (reloadList)
            {
                var request = WebRequest.Create(Program.STEAM_API_GAME_LIST_URL);
                request.Method = "GET";

                using var webResponse = request.GetResponse();
                using var webStream = webResponse.GetResponseStream();

                using var reader = new StreamReader(webStream);
                var data = await reader.ReadToEndAsync();

                using(StreamWriter sw = File.CreateText(filename))
                {
                    sw.WriteLine(data);
                }
            }

            var content = File.ReadAllText(filename);
            var apps = JsonConvert.DeserializeObject<GameInfo>(content).applist.apps;
            return apps.FirstOrDefault(x => x.appid.Equals(appId)).name ?? "Unknown Gamename";
        }

        public static async Task<SteamFileDetailJsonDetailModel> GetCollectionInfo(double steamCollectionId)
        {
            var model = await GetPublishedFileDetails<SteamFileDetailJsonModel>(new List<double> { steamCollectionId });
            return model.response.publishedfiledetails.FirstOrDefault();
        }
        #endregion

        #region Execute Steam Web Api
        private async void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e) => await UpdateSteamMods();
        public async Task UpdateSteamMods()
        {
            ConsoleExtensions.WriteColor(@"[//--Execute Refresh----------------------------------------------]", ConsoleColor.DarkGreen);

            if (Program.Settings.SteamCollection)
            {
                ConsoleExtensions.WriteColor(@$"[// ]Checking Steam collection with id {Program.Settings.SteamCollectionId}", ConsoleColor.DarkGreen);

                var parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("collectioncount", "1"));
                parameters.Add(new KeyValuePair<string, string>("publishedfileids[0]", Program.Settings.SteamCollectionId.ToString()));
                
                var httpClient = new HttpClient();

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_COLLECTION_URL, content);

                    var model = JsonConvert.DeserializeObject<SteamCollectionModel>(await response.Content.ReadAsStringAsync());
                    var modIds = model.response.collectiondetails.FirstOrDefault()?.children.Select(x => x.publishedfileid);

                    await CheckSteamMods(modIds.ToList());
                }
            }
            else
            {
                await CheckSteamMods(Program.Settings.SteamModIds);
            }
        }
        #endregion

        #region Check Mods for an Update
        private async Task CheckSteamMods(List<double> modIds)
        {
            ConsoleExtensions.WriteColor(@$"[// ]Checking {modIds.Count} Steam mods...", ConsoleColor.DarkGreen);

            var filename = "./SavedMods.json";
            var model = await GetPublishedFileDetails<SteamFileDetailJsonModel>(modIds);
            var needUpdateModels = new List<SteamFileDetailJsonDetailModel>();
            var savedMods = JsonConvert.DeserializeObject<List<LastEditModModel>>(File.ReadAllText(filename));

            foreach(var mod in model.response.publishedfiledetails)
            {
                var sMod = savedMods.FirstOrDefault(x => x.ModId.Equals(mod.publishedfileid.ToString()));
                if (sMod == null || !sMod.LastUpdate.ToString().Equals(mod.time_updated.ToString()))
                {
                    needUpdateModels.Add(mod);
                    if (sMod == null)
                    {
                        savedMods.Add(new LastEditModModel
                        {
                            ModId = mod.publishedfileid.ToString(),
                            LastUpdate = mod.time_updated.ToString()
                        });
                    }
                    else
                    {
                        savedMods.Find(x => x.ModId.Equals(mod.publishedfileid.ToString())).LastUpdate = mod.time_updated.ToString();
                    }
                }
            }

            File.WriteAllText(filename, JsonConvert.SerializeObject(savedMods));

            if(OnUpdatedModsFound != null)
            {
                OnUpdatedModsFound(this, new UpdatedModsEventArgs
                {
                    Mods = needUpdateModels
                });
            }
        }
        #endregion

        private static async Task<T> GetPublishedFileDetails<T>(List<double> files)
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

                HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_FILE_DETAILS_URL, content);

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
