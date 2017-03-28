    using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;

namespace SataniaBot.Modules
{
    [Name("Social")]
    public class SocialModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echos a message")]
        [Remarks("say hi")]
        public async Task Say([Remainder] string EchoMessage = null)
        {
            if(EchoMessage == null)
            {
                EchoMessage = "TRICKED";
            }

            await ReplyAsync(EchoMessage);
        }

        [Command("ping")]
        [Summary("Pong!")]
        [Remarks("ping")]
        public async Task Say()
        {
            await ReplyAsync("Pong!");
        }

        [Command("rng")]
        [Summary("Generates random number between 1-100")]
        [Remarks("rng")]
        public async Task Random()
        {
            Random rng = new Random();
            var RandomNumber = (rng.Next(1, 100));
            await ReplyAsync("Your random number is : " + $"{RandomNumber}");
        }

        [Command("love")]
        [Summary("\"Generates\" love between two people")]
        [Remarks("love tromo jessica")]
        public async Task Love(string User1, [Remainder] string User2 = null)
        {
            string PersonOne = User1;       //note to self: define most things outside for(),foreach() and while() loops unless the variable wont be needed
            string PersonTwo = User2;
            int SecondName = 0;
            int FirstName = 0;

            foreach (char Letter in PersonOne.ToLower())
            {
                FirstName = +Convert.ToInt32(Letter);
            };

            foreach (char Letter in PersonTwo.ToLower())      //for each makes the loop run once every one character in the string that is inputted
            {
                SecondName =+ Convert.ToInt32(Letter);          //converts the current letter and adds it to the SecondName Total
            }

            int Seed = SecondName + FirstName;

            Random rngseed = new Random(Seed);
            var LoveChance = (rngseed.Next(1, 100));

            await ReplyAsync("The Love between " + PersonOne + " and " + PersonTwo + " is " + $"{LoveChance}" + "%");
        }

        [Command("emote")]
        [Summary("Sends full size image of an emote")]
        [Remarks("emote meguButt")]
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

