using System.Collections.Generic;

namespace DiscordModNotifiyer.Models
{
    public class Settings
    {
        public bool Debug { get; set; }
        public string SteamApiKey { get; set; }
        public bool AutomaticRefresh { get; set; }
        public int AutomaticRefreshMin { get; set; }

        public string DiscordWebHook { get; set; }

        public bool SteamCollection { get; set; }
        public double SteamCollectionId { get; set; }
        public List<double> SteamModIds { get; set; }
    }
}
