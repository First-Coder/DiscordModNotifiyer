using System.Collections.Generic;

namespace DiscordModNotifiyer.Models
{
    public class SteamModJsonModel
    {
        public SteamModJsonResponseModel response { get; set; }
    }

    public class SteamModJsonResponseModel
    {
        public int result { get; set; }
        public int resultcount { get; set; }
        public List<SteamModJsonDetailModel> publishedfiledetails { get; set; }
    }

    public class SteamModJsonDetailModel
    {
        public double publishedfileid { get; set; }
        public int result { get; set; }
        public string creator { get; set; }
        public double creator_app_id { get; set; }
        public double consumer_app_id { get; set; }
        public string filename { get; set; }
        public double file_size { get; set; }
        public string file_url { get; set; }
        public string hcontent_file { get; set; }
        public string preview_url { get; set; }
        public string hcontent_url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public double time_created { get; set; }
        public double time_updated { get; set; }
        public bool visibility { get; set; }
        public bool banned { get; set; }
        public string ban_reason { get; set; }
        public double subscriptions { get; set; }
        public double favorited { get; set; }
        public double lifetime_subscriptions { get; set; }
        public double lifetime_favorited { get; set; }
        public double views { get; set; }
    }
}
