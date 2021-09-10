using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordModNotifiyer.Apis
{
    class SteamApi
    {
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

        #region Executen Steam Web Api
        private async void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e) => await UpdateSteamMods();
        public async Task UpdateSteamMods()
        {
            if (Program.Settings.SteamCollection)
            {
                var parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("collectioncount", "1"));
                parameters.Add(new KeyValuePair<string, string>("publishedfileids[0]", Program.Settings.SteamCollectionId.ToString()));
                Console.WriteLine(Program.STEAM_API_COLLECTION_URL);
                Console.WriteLine(Program.Settings.SteamCollectionId.ToString());
                var httpClient = new HttpClient();

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_COLLECTION_URL, content);

                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                var parameters = new List<KeyValuePair<string, string>>();
                parameters.Add(new KeyValuePair<string, string>("itemcount", "1"));

                int i = 0;
                foreach (var id in Program.Settings.SteamModIds)
                {
                    parameters.Add(new KeyValuePair<string, string>($"publishedfileids[{i++}]", id.ToString()));
                }

                var httpClient = new HttpClient();

                using (var content = new FormUrlEncodedContent(parameters))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.PostAsync(Program.STEAM_API_MOD_URL, content);

                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }
        #endregion
    }
}
