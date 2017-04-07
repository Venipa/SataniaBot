using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using System.Linq;
using Discord.WebSocket;

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
        public async Task Random(int MaxValue = 0, [Remainder] int MinValue = 0)
        {
            if (MaxValue < 0 || MinValue < 0)
            {
                await ReplyAsync("The number(s) have to be above 0");
                return;
            }

            if (MaxValue == 0)
            {
                MaxValue = 100;
            };

            if (MinValue> MaxValue)
            {
                MinValue = MaxValue - 1;
            };

            //await ReplyAsync("MinVal : "+$"{MinValue}"+" MaxVal : " + $"{MaxValue}");         test the numbers here
            Random rng = new Random();
            var RandomNumber = (rng.Next(MinValue,MaxValue));
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

        [Command("profile")]
        [Summary("Shows specified users profile or your own if unspecified")]
        [Remarks("profile tromodolo")]
        public async Task Profile(IGuildUser User = null)
        {
            if(User == null)
                User = Context.Message.Author as IGuildUser;

            var MarriedID = Satania.db.getMarriage(User.Id.ToString());
            bool isMarried;
            SocketUser MarriedPerson = null;
            if (MarriedID != null)
            {
                MarriedPerson = Satania._client.GetUser(205290788261724160);
                isMarried = true;
            }
            else
                isMarried = false;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = new Color(0, 0, 0);
            
            EmbedAuthorBuilder AuthorBuilder = new EmbedAuthorBuilder();
            AuthorBuilder.Name = User.Username;
            AuthorBuilder.IconUrl = User.GetAvatarUrl(AvatarFormat.Gif);
            if(isMarried == true)
                embed.ThumbnailUrl = MarriedPerson.GetAvatarUrl(AvatarFormat.Gif);
            embed.Author = AuthorBuilder;

            //username
            EmbedFieldBuilder NameField = new EmbedFieldBuilder();
            NameField.IsInline = true;
            NameField.Name = "Name";
            NameField.Value = User.Username + "#" +  User.Discriminator;
            embed.AddField(NameField);

            //Marriage
            EmbedFieldBuilder MarriedField = new EmbedFieldBuilder();
            MarriedField.IsInline = true;
            MarriedField.Name = "Married to:";
            if (isMarried == true)
                MarriedField.Value = MarriedPerson.Username + "#" + MarriedPerson.Discriminator;
            else
                MarriedField.Value = "No one";
            embed.AddField(MarriedField);

            //userid
            EmbedFieldBuilder UserIDField = new EmbedFieldBuilder();
            UserIDField.IsInline = true;
            UserIDField.Name = "User ID";
            UserIDField.Value = User.Id.ToString();
            embed.AddField(UserIDField);
            
            //account creation date
            EmbedFieldBuilder DateField = new EmbedFieldBuilder();
            DateField.IsInline = true;
            DateField.Name = "Account Creation Date";
            DateField.Value = User.CreatedAt.ToString("MMMM dd, yyyy");
            embed.AddField(DateField);


            /*      Out-commented for now because experience isnt up and running yet
            //level
            EmbedFieldBuilder LevelField = new EmbedFieldBuilder();
            LevelField.IsInline = true;
            LevelField.Name = "Level";
            LevelField.Value = "` 5023 `";
            embed.AddField(LevelField);

            //experience
            EmbedFieldBuilder ExpField = new EmbedFieldBuilder();
            ExpField.IsInline = true;
            ExpField.Name = "Experience";
            ExpField.Value = "` 271/382 `";
            embed.AddField(ExpField);
            */


            await ReplyAsync("", embed: embed);



        }


    }
}

