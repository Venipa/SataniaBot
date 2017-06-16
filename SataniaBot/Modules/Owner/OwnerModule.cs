using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SataniaBot.Services.EmbedExtensions;

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
        [Remarks("setavatar <URL>")]
        [RequireOwner()]
        public async Task SetAvatar(string AvatarURL = null)
        {
            if (AvatarURL == null)
                return;

            var http = new HttpClient();

            try
            {
                using (var stream = await http.GetStreamAsync(AvatarURL))
                {
                    var imgStream = new MemoryStream();
                    await stream.CopyToAsync(imgStream);
                    imgStream.Position = 0;
                    
                    await Satania._client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(imgStream)).ConfigureAwait(false);

                    await Context.Channel.SendConfirmAsync("Profile picture successfully changed :ok_hand:");
                }
            }
            catch
            {
                await Context.Channel.SendErrorAsync("Could not change profile picture to url, please try again later.");
            }
        }

        [Command("setgame")]
        [Summary("Sets game playing status")]
        [Remarks("setgame with fire")]
        [RequireOwner()]
        public async Task SetGame([Remainder]string Game = null)
        {
            if (Game == null)
                Game = "";

            await Context.Channel.SendConfirmAsync("Set game to: " + Game);
        }

        [Command("setname")]
        [Summary("Sets name of bot to name given")]
        [Remarks("setname Donald Trump")]
        [RequireOwner()]
        public async Task SetName([Remainder] string Name = null)
        {
            if (Name == null)
                return;

            try
            {
                await Satania._client.CurrentUser.ModifyAsync(x => x.Username = Name);

                await Context.Channel.SendConfirmAsync("Username successfully changed. :ok_hand:");
            }
            catch
            {
                await Context.Channel.SendErrorAsync("Something went wrong when trying to change username, please try again later.");
            }
        }
    }
}
