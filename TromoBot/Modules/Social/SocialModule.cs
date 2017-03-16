using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;

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
            var RandomNumber = (rng.Next(1, 100));
            await ReplyAsync("Your random number is : " + $"{RandomNumber}");
        }

        [Command("love")]
        public async Task Love()
        {
            Random rng = new Random();
            var LoveChance = (rng.Next(1, 100));
            var LoveReply = "you shouldn't see this";

            if (LoveChance <= 25)
            {
                LoveReply = "TromoBot doesn't love you :(";
            }

            else if (LoveChance >= 26  && LoveChance <= 50)
            {
                LoveReply = "TromoBot and you can be good friends :)";
            }

            else if (LoveChance >= 51 && LoveChance <= 75)
            {
                LoveReply = "TromoBot and you can be besties now :D";
            }

            else if (LoveChance >= 76)
            {
                LoveReply = "TromoBot loves you a lot <3";
            };

            await ReplyAsync(LoveReply);
        }

        [Command("emote")]
        public async Task Emote(string emote)
        {
            Regex r = new Regex("\\:(\\d.*?[1-9])\\>", RegexOptions.IgnoreCase); //using regex to match the id between the : and > in the emote code
            Match m = r.Match(emote);                                            //dont ask how regex works because i dont know
            if (m.Success)                                                       //black magic happens
            {
                String int1 = m.Groups[1].ToString();
                Console.WriteLine(int1);
                string imageurl = "https://cdn.discordapp.com/emojis/" + int1.ToString() + ".png";

                EmbedBuilder embed = new EmbedBuilder();
                embed.ImageUrl = imageurl;

                await ReplyAsync("", embed: embed);

            }
            else
            {
                return;
            }

        }
    }
}

