﻿using DiscordModNotifiyer.Events;
using DiscordModNotifiyer.Extensions;
using DiscordModNotifiyer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordModNotifiyer.Apis
{
    class SteamApi
    {
        /// <summary>
        /// Event if new mods are avalible
        /// </summary>
        public event EventHandler<UpdatedModsEventArgs> OnUpdatedModsFound;

        /// <summary>
        /// Time method for checking if new mods are avalible
        /// </summary>
        private System.Timers.Timer timer;

        #region Load / Refresh Timer
        /// <summary>
        /// Constructor
        /// </summary>
        public SteamApi() => RefreshSettings();

        /// <summary>
        /// Set the timer (if needed) for automatic check if new mods are avalible
        /// </summary>
        public void RefreshSettings()
        {
            timer = new System.Timers.Timer(Program.Settings.AutomaticRefreshMin * 60 * 1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timer.Enabled = Program.Settings.AutomaticRefresh;
        }
        #endregion

        #region Execute Steam Web Api
        /// <summary>
        /// Called timer event
        /// </summary>
        private async void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e) => await UpdateSteamMods();

        /// <summary>
        /// Get all mod ids by a given collection or over the Settings.json
        /// </summary>
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
        /// <summary>
        /// Check if the given mod ids got an update and call the event if needed
        /// </summary>
        /// <param name="modIds">Given mod ids</param>
        private async Task CheckSteamMods(List<double> modIds)
        {
            ConsoleExtensions.WriteColor(@$"[// ]Checking {modIds.Count} Steam mods...", ConsoleColor.DarkGreen);

            var filename = "./SavedMods.json";
            var model = await SteamExtensions.GetPublishedFileDetails<SteamFileDetailJsonModel>(modIds);
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
    }
}
