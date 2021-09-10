using DiscordModNotifiyer.Events;
using DiscordModNotifiyer.Extensions;
using DiscordModNotifiyer.Models;
using JNogueira.Discord.Webhook.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordModNotifiyer.Apis
{
    public class DiscordApi
    {
        public async Task SendHook(object sender, UpdatedModsEventArgs e)
        {
            ConsoleExtensions.WriteColor(@$"[// ]Send {e.Mods.Count} Steam mods to discord...", ConsoleColor.DarkGreen);

            // Get all Steam developer
            var developerSteamIds = new List<string>();
            foreach(var mod in e.Mods)
            {
                developerSteamIds.Add(mod.creator);
            }

            foreach (var mod in e.Mods)
            {
                ConsoleExtensions.WriteColor(@$"[// ]Send {mod.title} ({mod.publishedfileid}) to discord...", ConsoleColor.DarkGreen);

                SteamFileDetailJsonDetailModel collection;
                string collectionString = "";
                if (Program.Settings.SteamCollection)
                {
                    collection = await SteamApi.GetCollectionInfo(Program.Settings.SteamCollectionId);
                    collectionString = $" | Collection: {collection.title} (Id: {Program.Settings.SteamCollectionId})";
                }

                var players = await SteamApi.GetSteamPlayers(developerSteamIds);
                var gamename = await SteamApi.GetGameInfo(mod.creator_app_id);
                var client = new DiscordWebhookClient(Program.Settings.DiscordWebHook);
                var message = new DiscordMessage(
                    $"{gamename}{collectionString}\nMod: {mod.title} (Id: {mod.publishedfileid})",
                    username: $"{players.Find(x => x.steamid.Equals(mod.creator))?.personaname ?? mod.creator}",
                    avatarUrl: players.Find(x => x.steamid.Equals(mod.creator))?.avatar,
                    tts: false,
                    embeds: new[]
                    {
                        new DiscordMessageEmbed(
                            mod.title,
                            color: 0,
                            url: $"https://steamcommunity.com/sharedfiles/filedetails/?id={mod.publishedfileid}",
                            fields: new[]
                            {
                                new DiscordMessageEmbedField("Last Update", TimeExtensions.UnixTimeStampToDateTime(mod.time_updated).ToString()),
                                new DiscordMessageEmbedField("Created", TimeExtensions.UnixTimeStampToDateTime(mod.time_created).ToString()),
                                new DiscordMessageEmbedField("Views", mod.views.ToString()),
                                new DiscordMessageEmbedField("Subscriptions", mod.subscriptions.ToString())
                            },
                            thumbnail: new DiscordMessageEmbedThumbnail(mod.preview_url),
                            footer: new DiscordMessageEmbedFooter("(c) by L. Gmann")
                        )
                    }
                );
                await client.SendToDiscord(message);
            }

            ConsoleExtensions.WriteColor(@"[//---------------------------------------------------------------]", ConsoleColor.DarkGreen);
        }
    }
}
