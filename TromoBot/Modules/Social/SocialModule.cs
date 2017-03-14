using System;
using Discord.Commands;
using System.Threading.Tasks;

namespace TromoBot.Modules
{
    public class SocialModule : ModuleBase<SocketCommandContext>
    {

        [Command("test")]
        public async Task Say([Remainder] string echo)
        {

            await ReplyAsync(echo);

        }
        
    }
}
