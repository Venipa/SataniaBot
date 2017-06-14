using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SataniaBot.Services.EmbedExtensions;
using Discord.Addons.InteractiveCommands;

namespace SataniaBot.Modules
{
    [Name("Moderation")]
    public class ModerationModule : InteractiveModuleBase<SocketCommandContext>
    {
        
        [Command("setprefix")]
        [Summary("Sets server prefix")]
        [Remarks("s?setprefix ~")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefix(string Prefix = null)
        {
            if (Prefix == null)
            {
                await Context.Channel.SendErrorAsync("You need to specify a prefix at least 1 character long.");
            }
            else if (Prefix.Length > 10)
            {
                await Context.Channel.SendErrorAsync("The prefix can't be longer than 10 characters long.");
            }
            else
            {
                Satania.db.updatePrefix(Context.Guild, Prefix);
                await Context.Channel.SendConfirmAsync("New prefix set to: `" + Prefix + "`");
            }
        }

        [Command("ban", RunMode = RunMode.Async)]
        [Summary("Bans a user from the server and deletes last day of messages")]
        [Remarks("s?ban reddeyez")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser BanUser = null, [Remainder]string reason = null)
        {
            if (BanUser == null)
            {
                await Context.Channel.SendErrorAsync("You need to specify a user to ban.");
            }
            else if (BanUser == Context.Message.Author)
            {
                await Context.Channel.SendErrorAsync("You can't ban yourself.");
            }
            else if (BanUser.Hierarchy == Int32.MaxValue)
            {
                await Context.Channel.SendErrorAsync("You can't ban the owner of a server.");
            }
            else if (BanUser.Hierarchy >= (Context.Message.Author as SocketGuildUser).Hierarchy)
            {
                await Context.Channel.SendErrorAsync("You can't ban someone with a role higher or equal to yours.");
            }
            else
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(reason))
                        reason = "None";

                    var time = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc);

                    await Context.Channel.SendColouredEmbedAsync($"Are you sure you want to ban user: {BanUser}?", $"**Send `confirm` or `cancel`**\n**Date:** {time} UTC \n**Reason:** {reason}", new Color());
                    var response = await WaitForMessage(Context.Message.Author, Context.Channel, new TimeSpan(0, 0, 60));
                    if (response.Content.ToString().ToLower() == "confirm")
                    {

                        var BanDM = await BanUser.CreateDMChannelAsync();

                        await BanDM.SendErrorAsync($"You were banned from: {Context.Guild.Name}", $"**Date:** {time} UTC \n" +
                                                                                                  $"**Reason:** {reason}");
                        await Context.Guild.AddBanAsync(BanUser, 1);

                        await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}\n {BanUser} was banned from this server.");
                    }
                    else
                    {
                        await Context.Channel.SendErrorAsync($"User {BanUser} was not banned.");
                    }    
                }
                catch
                {
                    await Context.Channel.SendErrorAsync($"{Context.Message.Author.Mention}\n {BanUser} could not be banned from this server.");
                }
            }
        }

        [Command("kick", RunMode = RunMode.Async)]
        [Summary("Kicks a user from the server")]
        [Remarks("s?kick kbuns")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser KickUser = null, [Remainder]string reason = null)
        {
            if (KickUser == null)
            {
                await Context.Channel.SendErrorAsync("You need to specify a user to kick.");
            }
            else if (KickUser == Context.Message.Author)
            {
                await Context.Channel.SendErrorAsync("You can't kick yourself.");
            }
            else if (KickUser.Hierarchy == Int32.MaxValue)
            {
                await Context.Channel.SendErrorAsync("You can't kick the owner of a server.");
            }
            else if (KickUser.Hierarchy >= (Context.Message.Author as SocketGuildUser).Hierarchy)
            {
                await Context.Channel.SendErrorAsync("You can't kick someone with a role higher or equal to yours.");
            }
            else {
                try
                {
                    if (string.IsNullOrWhiteSpace(reason))
                        reason = "None";

                    var time = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc);

                    await Context.Channel.SendColouredEmbedAsync($"Are you sure you want to kick user: {KickUser}?", $"**Send `confirm` or `cancel`**\n**Date:** {time} UTC \n**Reason:** {reason}", new Color());
                    var response = await WaitForMessage(Context.Message.Author, Context.Channel, new TimeSpan(0, 0, 60));
                    if (response.Content.ToString().ToLower() == "confirm")
                    {
                        var KickDM = await KickUser.CreateDMChannelAsync();

                        await KickDM.SendErrorAsync($"You were banned from: {Context.Guild.Name}", $"**Date:** {time} UTC \n" +
                                                                                                   $"**Reason:** {reason}");

                        await KickUser.KickAsync();

                        await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}\n {KickUser} was kicked from this server.");
                    }
                    else
                    {
                        await Context.Channel.SendErrorAsync($"User {KickUser} was not banned.");
                    }
                }
                catch
                {
                    await Context.Channel.SendErrorAsync($"{Context.Message.Author.Mention}\n {KickUser} couldn't be kicked from this server.");
                }
            }
        }

        [Command("prune")]
        [Summary("Prunes number of messages you want, up to 100")]
        [Remarks("s?prune 27")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Prune(int PruneNumber = 10)
        {
            if (PruneNumber > 100)
            {
                PruneNumber = 100;
            }

            var Messages = (await Context.Channel.GetMessagesAsync(PruneNumber + 1).Flatten().ConfigureAwait(false));
            if (Messages.FirstOrDefault()?.Id == Context.Message.Id)
                Messages = Messages.Skip(1).ToArray();
            else
                Messages = Messages.Take(PruneNumber);
            await Context.Channel.DeleteMessagesAsync(Messages).ConfigureAwait(false);

            await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}\n{PruneNumber} messages were pruned");
        }


        [Command("togglensfw")]
        [Summary("Sets a certain channel to be a nsfw channel, allowing nsfw commands in it")]
        [Remarks("s?togglensfw")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ToggleNSFW()
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                Satania.db.removeNsfwChannel(Context.Channel.Id.ToString());
                await Context.Channel.SendErrorAsync("Channel has been removed as an NSFW channel.");
            }
            else
            {
                Satania.db.addNsfwChannel(Context.Channel.Id.ToString());
                await Context.Channel.SendConfirmAsync("Channel has been added as an NSFW channel.");
            }
        }
    }
}
