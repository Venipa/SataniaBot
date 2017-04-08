using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using System.Linq;
using Discord.WebSocket;
using Discord.Addons.InteractiveCommands;

namespace SataniaBot.Modules
{
    [Name("Social")]
    public class SocialModule : InteractiveModuleBase<SocketCommandContext>
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

            string MarriedID = Satania.db.getMarriage(User.Id.ToString());
            bool isMarried;
            SocketUser MarriedPerson = null;
            if (!string.IsNullOrWhiteSpace(MarriedID))
            {
                MarriedPerson = Satania._client.GetUser(Convert.ToUInt64(MarriedID));
                isMarried = true;
            }
            else
                isMarried = false;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = new Color(190, 67, 224);

            EmbedAuthorBuilder AuthorBuilder = new EmbedAuthorBuilder();
            AuthorBuilder.Name = User.Username;
            AuthorBuilder.IconUrl = User.GetAvatarUrl();
            if(isMarried == true)
                embed.ThumbnailUrl = MarriedPerson?.GetAvatarUrl();
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
                MarriedField.Value = MarriedPerson?.Username + "#" + MarriedPerson?.Discriminator;
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


        [Command("marry", RunMode = RunMode.Async)]
        [Summary("Proposes to marry to specified person")]
        [Remarks("marry tromodolo")]
        public async Task Marry(IGuildUser Proposal = null)
        {
            if (Proposal == null)
            {
                await ReplyAsync("You need to specify a person to marry.");
            }
            else if (Satania.db.getMarriage(Context.Message.Author.Id.ToString()) == Proposal.Id.ToString())
            {
                await ReplyAsync("You are already married to that person");
            }
            else if (!string.IsNullOrWhiteSpace(Satania.db.getMarriage(Context.Message.Author.Id.ToString())))
            {
                await ReplyAsync("You are already married, polygamy is a bad bad. If you want to marry someone else you have to divorce your partner.");
            }
            else if (!string.IsNullOrWhiteSpace(Satania.db.getMarriage(Proposal.Id.ToString())))
            {
                await ReplyAsync("You can't marry someone who is already married.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(Proposal.Mention + ", " + Context.Message.Author.Mention + " wants to marry you. Write `y/yes` to accept or `n/no` to decline. You have a minute to respond.");
                var response = await WaitForMessage(Proposal, Context.Channel, new TimeSpan(0, 0, 60), new MessageContainsResponsePrecondition("y", "yes", "no", "n"));

                if (response.Content.ToString().ToLower() == "y" || response.Content.ToString().ToLower() == "yes")
                {
                    await ReplyAsync("The person accepted your proposal. :heart:");
                    Satania.db.addMarriage(Context.Message.Author.Id.ToString(), Proposal.Id.ToString());
                }
                else if(response.Content.ToString().ToLower() == "n" || response.Content.ToString().ToLower() == "no")
                {
                    await ReplyAsync("You got denied. :frowning:");
                }
                
            }
        }

        [Command("divorce", RunMode = RunMode.Async)]
        [Summary("Asks to divorce from specified person if married")]
        [Remarks("divorce tromodolo")]
        public async Task Divorce(IGuildUser Proposal = null)
        {
            if (Proposal == null)
            {
                await ReplyAsync("You need to specify a person to divorce.");
            }
            else if (string.IsNullOrWhiteSpace(Satania.db.getMarriage(Context.Message.Author.Id.ToString()))) //If column with married is empty for author
            {
                await ReplyAsync("You aren't married to anyone.");
            }
            else if (string.IsNullOrWhiteSpace(Satania.db.getMarriage(Proposal.Id.ToString()))) // if column with married is empty for target, which should only happen if author isnt empty
            {
                await ReplyAsync("You aren't married to that person.");
            }
            else if (Context.Message.Author.Id.ToString() != Satania.db.getMarriage(Proposal.Id.ToString())) // should trigger if neither person has empty columns but the targets value doesnt match author id
            {
                await ReplyAsync("That person isnt married to you.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(Proposal.Mention + ", " + Context.Message.Author.Mention + " wants to divorce you. Write `y/yes` to accept or `n/no` to decline. You have a minute to respond.");
                var response = await WaitForMessage(Proposal, Context.Channel, new TimeSpan(0, 0, 60), new MessageContainsResponsePrecondition("y", "yes", "no", "n"));

                if (response.Content.ToString().ToLower() == "y" || response.Content.ToString().ToLower() == "yes")
                {
                    await ReplyAsync("The person accepted the divorce.");
                    Satania.db.removeMarriage(Context.Message.Author.Id.ToString(), Proposal.Id.ToString());
                }
                else if (response.Content.ToString().ToLower() == "n" || response.Content.ToString().ToLower() == "no")
                {
                    await ReplyAsync("The person didn't accept the divorce. You are still married.");
                }
            }

        }

    }
}

