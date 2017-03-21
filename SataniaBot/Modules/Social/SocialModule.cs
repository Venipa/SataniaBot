using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;

namespace SataniaBot.Modules
{
    public class SocialModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        public async Task Say([Remainder] string echo = null)
        {
            if(echo == null)
            {
                echo = "TRICKED";
            }

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
        public async Task Love(string user1, [Remainder] string user2 = null)
        {
            string PersonOne = user1;       //note to self: define most things outside for(),foreach() and while() loops unless the variable wont be needed
            string PersonTwo = user2;
            int SecondName = 0;
            int FirstName = 0;

            foreach (char Letter in PersonOne)
            {
                FirstName = +Convert.ToInt32(Letter);
            };

            foreach (char Letter in PersonTwo)      //for each makes the loop run once every one character in the string that is inputted
            {
                SecondName =+ Convert.ToInt32(Letter);          //converts the current letter and adds it to the SecondName Total
            }

            await ReplyAsync($"{FirstName}, {SecondName}");
            int Seed = SecondName + FirstName;

            Random rngseed = new Random(Seed);
            var LoveChance = (rngseed.Next(1, 100));

            await ReplyAsync("The Love between " + PersonOne + " and " + PersonTwo + " is " + $"{LoveChance}" + "%");
        }

        [Command("emote")]
        public async Task Emote(string emote)
        {
            Regex r = new Regex("\\:(\\d.*?[0-9])\\>", RegexOptions.IgnoreCase); //using regex to match the id between the : and > in the emote code
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

