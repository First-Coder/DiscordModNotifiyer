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
            timer = new System.Timers.Timer(Program.Settings.AutomaticRefreshMin * 1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = Program.Settings.AutomaticRefresh;
        }
        #endregion

        public static async Task<SteamPlayerPlayerModel> GetSteamPlayer(string steamid)
        {
            var request = WebRequest.Create(Program.STEAM_API_PLAYER_URL + Program.Settings.SteamApiKey + "&steamids=" + steamid.ToString());
            request.Method = "GET";

            using var webResponse = request.GetResponse();
            using var webStream = webResponse.GetResponseStream();

            using var reader = new StreamReader(webStream);
            var data = await reader.ReadToEndAsync();
            var model = JsonConvert.DeserializeObject<SteamPlayerModel>(data);

            return model.response.players.FirstOrDefault();
        }

        #region Executen Steam Web Api
        private async void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e) => await UpdateSteamMods();
        public async Task UpdateSteamMods()
        {
            if (Program.Settings.SteamCollection)
            {
                ConsoleExtensions.WriteColor(@$"[// ]Checking Steam collection with id {Program.Settings.SteamCollectionId}", ConsoleColor.DarkGreen);

                var parameters = new List<KeyValuePair<string, string>>();
                //parameters.Add(new KeyValuePair<string, string>("itemcount", "1"));
                parameters.Add(new KeyValuePair<string, string>("collectioncount", "1"));
                parameters.Add(new KeyValuePair<string, string>("publishedfileids[0]", Program.Settings.SteamCollectionId.ToString()));
                
                var httpClient = new HttpClient();

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_COLLECTION_URL, content);

                    //Console.WriteLine(await response.Content.ReadAsStringAsync());
                    //return;

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

            var parameters = new List<KeyValuePair<string, string>>();
            int i = 0;
            foreach (var id in modIds)
            {
                parameters.Add(new KeyValuePair<string, string>($"publishedfileids[{i++}]", id.ToString()));
            }
            parameters.Add(new KeyValuePair<string, string>("itemcount", i.ToString()));

            var httpClient = new HttpClient();

            using (var content = new FormUrlEncodedContent(parameters))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_MOD_URL, content);

                var model = JsonConvert.DeserializeObject<SteamModJsonModel>(await response.Content.ReadAsStringAsync());
                if(OnUpdatedModsFound != null)
                {
                    OnUpdatedModsFound(this, new UpdatedModsEventArgs
                    {
                        Mods = model.response.publishedfiledetails
                    });
                }
            }
        }
        #endregion
    }
}
