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
    [Name("Owner")]
    public class OwnerModule : ModuleBase<SocketCommandContext>
    {

        [Command("die")]
        [Summary("Tactical kill switch")]
        [Remarks("die")]
        [RequireOwner()]
        public async Task Die()
        {
            await ReplyAsync("Sorry if I did anything wrong. :(");

            Environment.Exit(0);

        }

        [Command("setavatar")]
        [Summary("Sets avatar to image url")]
        [Remarks("setavatar URL")]
        [RequireOwner()]
        public async Task SetAvatar(string AvatarURL = null)
        {
            if (AvatarURL == null)
                return;

            EmbedBuilder builder = new EmbedBuilder();
            var http = new HttpClient();

            try
            {
                using (var stream = await http.GetStreamAsync(AvatarURL))
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

        [Command("setname")]
        [Summary("Sets name of bot to name given")]
        [Remarks("setname Donald Trump")]
        [RequireOwner()]
        public async Task SetName([Remainder] string Name = null)
        {

            EmbedBuilder builder = new EmbedBuilder();

            if (Name == null)
                return;

            try
            {
                await Satania._client.CurrentUser.ModifyAsync(x => x.Username = Name);

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
    }
}
