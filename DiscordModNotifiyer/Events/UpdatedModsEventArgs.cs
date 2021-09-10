using DiscordModNotifiyer.Models;
using System;
using System.Collections.Generic;

namespace DiscordModNotifiyer.Events
{
    public class UpdatedModsEventArgs : EventArgs
    {
        public List<SteamModJsonDetailModel> Mods { get; set; }
    }
}
