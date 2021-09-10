using DiscordModNotifiyer.Events;
using DiscordModNotifiyer.Extensions;
using JNogueira.Discord.Webhook.Client;
using System;
using System.Threading.Tasks;

namespace DiscordModNotifiyer.Apis
{
    public class DiscordApi
    {
        public async Task SendHook(object sender, UpdatedModsEventArgs e)
        {
            ConsoleExtensions.WriteColor(@$"[// ]Send {e.Mods.Count} Steam mods to discord...", ConsoleColor.DarkGreen);

            foreach (var mod in e.Mods)
            {
                ConsoleExtensions.WriteColor(@$"[// ]Send {mod.title} ({mod.publishedfileid}) to discord...", ConsoleColor.DarkGreen);

                var player = await SteamApi.GetSteamPlayer(mod.creator);
                var client = new DiscordWebhookClient(Program.Settings.DiscordWebHook);
                var message = new DiscordMessage(
                    $"We got a new Mod Update for the Mod: {mod.title} ({mod.publishedfileid}) \n",
                    username: $"{player.personaname} ({mod.creator})",
                    avatarUrl: player.avatar,
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
