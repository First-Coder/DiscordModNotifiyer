using System.Collections.Generic;

namespace DiscordModNotifiyer.Models
{
    public class GameInfo
    {
        public GameInfoApps applist { get; set; }
    }

    public class GameInfoApps
    {
        public List<GameInfoDetail> apps { get; set; }
    }

    public class GameInfoDetail
    {
        public double appid { get; set; }
        public string name { get; set; }
    }
}
