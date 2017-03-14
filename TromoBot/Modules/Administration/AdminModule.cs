using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TromoBot;

namespace TromoBot.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {

        [Command("setname")]
        [Alias("setnick")]
        [RequireOwner()]
        public async Task SetName([Remainder] string name = null)
        {
            if (name == null)
                return;

            try { 
                await TromoBot._client.CurrentUser.ModifyAsync(x => x.Username = name);
                await ReplyAsync("Username successfully changed. :ok_hand:");
            }
            catch
            {   
                await ReplyAsync("Something went wrong when trying to change username, please try again later.");
            }
        }

        [Command("setavatar")]
        [RequireOwner()]
        public async Task SetAvatar(string url = null)
        {
            if (url == null)
                return;

            var http = new HttpClient();

            using (var stream = await http.GetStreamAsync(url))
            {
                var imgStream = new MemoryStream();
                await stream.CopyToAsync(imgStream);
                imgStream.Position = 0;

                await TromoBot._client.CurrentUser.ModifyAsync(u => u.Avatar = new Image(imgStream)).ConfigureAwait(false);
                await ReplyAsync("Profile picture successfully changed :ok_hand:");
            }
            
        }

    }
}
