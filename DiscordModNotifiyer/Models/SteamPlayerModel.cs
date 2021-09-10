using System.Collections.Generic;

namespace DiscordModNotifiyer.Models
{
    public class SteamPlayerModel
    {
        public SteamPlayerResponseModel response { get; set; }
    }

    public class SteamPlayerResponseModel
    {
        public List<SteamPlayerPlayerModel> players { get; set; }
    }

    public class SteamPlayerPlayerModel
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilstate { get; set; }
        public string personaname { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string avatarhash { get; set; }
        public int personastate { get; set; }
        public string primaryclanid { get; set; }
        public double timecreated { get; set; }
        public int personastateflags { get; set; }
    }
}
