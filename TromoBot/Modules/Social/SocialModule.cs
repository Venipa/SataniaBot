using System;
using Discord.Commands;
using System.Threading.Tasks;

namespace TromoBot.Modules
{
    public class SocialModule : ModuleBase<SocketCommandContext>
    {

        [Command("say")]
        public async Task Say([Remainder] string echo)
        {
            await ReplyAsync(echo);
        }

        [Command("ping")]
        public async Task Say()
        {
            await ReplyAsync("Pong!");
        }

        [Command("rng")]
        public async Task Random()
        {
            Random rng = new Random();
            var RandomNumber = (rng.Next(1, 101));
            await ReplyAsync("Your random number is : "+$"{RandomNumber}");
        }
    }
}

