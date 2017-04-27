using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using SataniaBot.Services.EmbedExtensions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SataniaBot.Modules
{
    [Name("Fun")]
    public class FunModule : InteractiveModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echos a message")]
        [Remarks("s?say hi")]
        public async Task Say([Remainder] string EchoMessage = null)
        {
            if (EchoMessage == null)
            {
                EchoMessage = "TRICKED";
            }

            await ReplyAsync(EchoMessage);
        }

        [Command("ping")]
        [Summary("Pong!")]
        [Remarks("s?ping")]
        public async Task Say()
        {
            await ReplyAsync("Pong!");
        }

        [Command("rng")]
        [Summary("Generates random number between 1-100")]
        [Remarks("s?rng")]
        public async Task Random(int MaxValue = 0, int MinValue = 0)
        {
            if (MaxValue < 0 || MinValue < 0)
            {
                await Context.Channel.SendErrorAsync("The number(s) have to be above 0");
                return;
            }

            if (MaxValue == 0)
            {
                MaxValue = 100;
            };

            if (MinValue > MaxValue)
            {
                MinValue = MaxValue - 1;
            };

            //await ReplyAsync("MinVal : "+$"{MinValue}"+" MaxVal : " + $"{MaxValue}");         test the numbers here
            Random rng = new Random();
            var RandomNumber = (rng.Next(MinValue, MaxValue));
            await Context.Channel.SendConfirmAsync("Your random number is : " + $"{RandomNumber}");
        }

        [Command("love")]
        [Summary("\"Generates\" love between two people")]
        [Remarks("s?love tromo life")]
        public async Task Love(string User1, [Remainder] string User2 = null)
        {
            if ((User1.ToLower().Contains("tromo") && User2.ToLower().Contains("kei")) || (User2.ToLower().Contains("tromo") && User1.ToLower().Contains("kei")))
            {
                await Context.Channel.SendColouredEmbedAsync("The love between Tromo and Kei is 100%", new Color(219, 91, 255));
            }
            else
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
                    SecondName = +Convert.ToInt32(Letter);          //converts the current letter and adds it to the SecondName Total
                }

                int Seed = SecondName + FirstName;

                Random rngseed = new Random(Seed);
                var LoveChance = (rngseed.Next(1, 100));
                // original color value is (219, 91, 255) 

                double Rcolor = 219 / 100 * LoveChance;
                double Gcolor = 91 / 100 * LoveChance;
                double Bcolor = 255 / 100 * LoveChance;

                //Console.WriteLine(Rcolor + "," + Gcolor + "," + Bcolor); 
                //await ReplyAsync($"{RcolorFound}" + $",{GcolorFound}" + $",{BcolorFound}"); 

                Color LoveColor = new Color((byte)Rcolor, (byte)Gcolor, (byte)Bcolor);

                await Context.Channel.SendColouredEmbedAsync("The Love between " + PersonOne + " and " + PersonTwo + " is " + $"{LoveChance}" + "%", LoveColor);
            }
        }

        [Command("emote")]
        [Summary("Sends full size image of an emote")]
        [Remarks("s?emote meguButt")]
        public async Task Emote(string emote)
        {
            Regex r = new Regex("\\:(\\d.*?[0-9])\\>", RegexOptions.IgnoreCase); //using regex to match the id between the : and > in the emote code
            Match m = r.Match(emote);                                            //dont ask how regex works because i dont know
            if (m.Success)                                                       //black magic happens
            {
                String int1 = m.Groups[1].ToString();
                string imageurl = "https://cdn.discordapp.com/emojis/" + int1.ToString() + ".png";

                await Context.Channel.SendImageEmbedAsync(imageurl);
            }
            else
            {
                return;
            }

        }
    }
}

