using DiscordModNotifiyer.Events;
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
                    string jsonContent = "Content was not parsed";

                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    try
                    {
                        HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_COLLECTION_URL, content);

                        jsonContent = await response.Content.ReadAsStringAsync();
                        var model = JsonConvert.DeserializeObject<SteamCollectionModel>(jsonContent);
                        var modIds = model.response.collectiondetails.FirstOrDefault()?.children.Select(x => x.publishedfileid);

                        await CheckSteamMods(modIds.ToList());
                    }
                    catch (Exception e)
                    {
                        ConsoleExtensions.Error(e.Message, Program.Settings.Debug ? $"Json Content: {jsonContent}" : "");
                        return;
                    }
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
            var model = await SteamExtensions.GetPublishedFileDetails(modIds);
            var needUpdateModels = new List<SteamFileDetailJsonDetailModel>();

            List<LastEditModModel> savedMods;
            try
            {
                savedMods = JsonConvert.DeserializeObject<List<LastEditModModel>>(File.ReadAllText(filename));
            }
            catch(Exception e)
            {
                ConsoleExtensions.CriticalError(e.Message, 2);
                return;
            }

            // Http request failed. No check possible
            if (model == null)
            {
                return;
            }

            foreach (var mod in model.response.publishedfiledetails)
            {
                var sMod = savedMods.FirstOrDefault(x => x.ModId.Equals(mod.publishedfileid.ToString()));
                if (sMod == null || !sMod.LastUpdate.ToString().Equals(mod.time_updated.ToString()))
                {
                    needUpdateModels.Add(mod);
                    if (sMod == null)
                    {
                        if(Program.Settings.Debug)
                        {
                            ConsoleExtensions.WriteColor($"[// Debug ]Mod Id \"{mod.publishedfileid}\" is not found in SavedMod.json file. Set them now to needed update", ConsoleColor.Yellow);
                        }
                        savedMods.Add(new LastEditModModel
                        {
                            ModId = mod.publishedfileid.ToString(),
                            LastUpdate = mod.time_updated.ToString()
                        });
                    }
                    else
                    {
                        if(Program.Settings.Debug)
                        {
                            ConsoleExtensions.WriteColor($"[// Debug ]Mod Id {sMod.ModId} \"{mod.publishedfileid}\" need a update cause the timestamps are not even ({sMod.LastUpdate} != {mod.time_updated})", ConsoleColor.Yellow);
                        }
                        savedMods.Find(x => x.ModId.Equals(mod.publishedfileid.ToString())).LastUpdate = mod.time_updated.ToString();
                    }
                }
            }

            File.WriteAllText(filename, JsonConvert.SerializeObject(savedMods));

            if (OnUpdatedModsFound != null && needUpdateModels.Count > 0)
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
