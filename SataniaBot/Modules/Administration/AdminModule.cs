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
    [Name("Admin")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {

        [Command("setname")]
        [Alias("setnick")]
        [RequireOwner()]
        public async Task SetName([Remainder] string name = null)
        {

            EmbedBuilder builder = new EmbedBuilder();

            if (name == null)
                return;

            try { 
                await Satania._client.CurrentUser.ModifyAsync(x => x.Username = name);

                builder.Description = "Username successfully changed. :ok_hand:";
                builder.Color = new Color(111, 237, 69);

                await ReplyAsync("", embed: builder);
            }
            catch
            {
                builder.Description = "Something went wrong when trying to change username, please try again later.";
                builder.Color = new Color(222, 90, 47);

                await ReplyAsync("", embed: builder);
            }
        }

        [Command("setavatar")]
        [RequireOwner()]
        public async Task SetAvatar(string url = null)
        {
            if (url == null)
                return;

            EmbedBuilder builder = new EmbedBuilder();
            var http = new HttpClient();

            try
            {
                using (var stream = await http.GetStreamAsync(url))
                {
                    var imgStream = new MemoryStream();
                    await stream.CopyToAsync(imgStream);
                    imgStream.Position = 0;

                    await Satania._client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(imgStream)).ConfigureAwait(false);

                    builder.Description = "Profile picture successfully changed :ok_hand:";
                    builder.Color = new Color(111, 237, 69);

                    await ReplyAsync("", embed: builder);
                }
            }
            catch
            {
                builder.Description = "Could not change profile picture to url, please try again later.";
                builder.Color = new Color(222, 90, 47);

                await ReplyAsync("", embed: builder);
            }
        }

        [Command("setprefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefix(string prefix)
        {
            Satania.db.updatePrefix(Context.Guild, prefix);
            await ReplyAsync("New prefix set to: " + prefix);
        }


        [Command("die")]
        [RequireOwner()]
        public async Task Die()
        {
            await ReplyAsync("Sorry if I did anything wrong. :(");

            Environment.Exit(0);

        }

        [Command("prune")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Prune(int number = 10)
        {
            EmbedBuilder builder = new EmbedBuilder();
            var msgs = Context.Channel.GetCachedMessages(number + 1);       //only gets cached messages at the moment, will get fixed

            await Context.Channel.DeleteMessagesAsync(msgs);

            builder.Description = $"{Context.Message.Author.Mention}\n{number} messages were pruned";
            builder.Color = new Color(111, 237, 69);
            
            await ReplyAsync("", embed: builder);
           
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user = null)
        {

            EmbedBuilder builder = new EmbedBuilder();

            try
            {
                await user.KickAsync();

                builder.Description = $"{Context.Message.Author.Mention}\n {user} was kicked from this server.";
                builder.Color = new Color(111, 237, 69);

                await ReplyAsync("", embed: builder);
            }
            catch
            {
                builder.Description = $"{Context.Message.Author.Mention}\n {user} could not be kicked from this server.";
                builder.Color = new Color(222, 90, 47);

                await ReplyAsync("", embed: builder);
            }

        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser user = null)
        {

            EmbedBuilder builder = new EmbedBuilder();

            try
            {
                await Context.Guild.AddBanAsync(user, 1);

                builder.Description = $"{Context.Message.Author.Mention}\n {user} was banned from this server.";
                builder.Color = new Color(111, 237, 69);

                await ReplyAsync("", embed: builder);
            }
            catch
            {
                builder.Description = $"{Context.Message.Author.Mention}\n {user} could not be banned from this server.";
                builder.Color = new Color(222, 90, 47);

                await ReplyAsync("", embed: builder);
            }
        }
    }
}
