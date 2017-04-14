using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
            await ReplyAsync("New prefix set to: " + Prefix);
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

                builder.Description = $"{Context.Message.Author.Mention}\n {BanUser} was banned from this server.";
                builder.Color = new Color(111, 237, 69);

                await ReplyAsync("", embed: builder);
            }
            catch
            {
                builder.Description = $"{Context.Message.Author.Mention}\n {BanUser} could not be banned from this server.";
                builder.Color = new Color(222, 90, 47);

                await ReplyAsync("", embed: builder);
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

                builder.Description = $"{Context.Message.Author.Mention}\n {KickUser} was kicked from this server.";
                builder.Color = new Color(111, 237, 69);

                await ReplyAsync("", embed: builder);
            }
            catch
            {
                builder.Description = $"{Context.Message.Author.Mention}\n {KickUser} could not be kicked from this server.";
                builder.Color = new Color(222, 90, 47);

                await ReplyAsync("", embed: builder);
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
            var msgs = Context.Channel.GetCachedMessages(PruneNumber + 1);       //only gets cached messages at the moment, will get fixed

            await Context.Channel.DeleteMessagesAsync(msgs);

            builder.Description = $"{Context.Message.Author.Mention}\n{PruneNumber} messages were pruned";
            builder.Color = new Color(111, 237, 69);

            await ReplyAsync("", embed: builder);

        }

    }
}
