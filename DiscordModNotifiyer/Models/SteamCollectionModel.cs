using System.Collections.Generic;

namespace DiscordModNotifiyer.Models
{
    public class SteamCollectionModel
    {
        public SteamCollectionResponseModel response { get; set; }
    }

    public class SteamCollectionResponseModel
    {
        public int result { get; set; }
        public int resultcount { get; set; }
        public List<SteamCollectionDetailModel> collectiondetails { get; set; }
    }

    public class SteamCollectionDetailModel
    {
        public double publishedfileid { get; set; }
        public int result { get; set; }
        public List<SteamCollectionChildren> children { get; set; }
    }

    public class SteamCollectionChildren
    {
        public double publishedfileid { get; set; }
        public int sortorder { get; set; }
        public int filetype { get; set; }
    }
}
