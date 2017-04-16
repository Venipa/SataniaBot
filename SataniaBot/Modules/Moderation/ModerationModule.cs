﻿using Discord;
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

namespace SataniaBot.Modules
{
    [Name("Moderation")]
    public class ModerationModule : ModuleBase<SocketCommandContext>
    {
        
        [Command("setprefix")]
        [Summary("Sets server prefix")]
        [Remarks("#setprefix ~")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefix(string Prefix)
        {
            Satania.db.updatePrefix(Context.Guild, Prefix);
            await ReplyAsync("New prefix set to: `" + Prefix + "`");
        }

        [Command("ban")]
        [Summary("Bans a user from the server and deletes last week of messages")]
        [Remarks("#ban reddeyez")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser BanUser = null)
        {

            EmbedBuilder builder = new EmbedBuilder();

            try
            {
                await Context.Guild.AddBanAsync(BanUser, 1);

                await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}\n {BanUser} was banned from this server.");
            }
            catch
            {
                await Context.Channel.SendErrorAsync($"{Context.Message.Author.Mention}\n {BanUser} was banned from this server.");
            }
        }

        [Command("kick")]
        [Summary("Kicks a user from the server")]
        [Remarks("#kick kbuns")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser KickUser = null)
        {

            EmbedBuilder builder = new EmbedBuilder();

            try
            {
                await KickUser.KickAsync();

                await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}\n {KickUser} was kicked from this server.");
            }
            catch
            {
                await Context.Channel.SendErrorAsync($"{Context.Message.Author.Mention}\n {KickUser} was kicked from this server.");
            }
        }

        [Command("prune")]
        [Summary("Prunes number of messages you want, up to 100")]
        [Remarks("#prune 27")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Prune(int PruneNumber = 10)
        {
            EmbedBuilder builder = new EmbedBuilder();

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

    }
}
