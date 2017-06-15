using System;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord;
using System.Linq;
using Discord.WebSocket;
using Discord.Addons.InteractiveCommands;
using SataniaBot.Services.EmbedExtensions;

namespace SataniaBot.Modules
{
    [Name("Social")]
    public class SocialModule : InteractiveModuleBase<SocketCommandContext>
    {
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
            NameField.Name = "Name:";
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

            //level
            var userstats = Satania.db.getLevel(User.Id.ToString());
            EmbedFieldBuilder LevelField = new EmbedFieldBuilder();
            LevelField.IsInline = true;
            LevelField.Name = "Level " + userstats.level;
            LevelField.Value = $"`{userstats.currentExp}/{userstats.levelExp} XP`";
            embed.AddField(LevelField);

            //experience
            EmbedFieldBuilder ExpField = new EmbedFieldBuilder();
            ExpField.IsInline = true;
            ExpField.Name = "Total Experience:";
            ExpField.Value = $"` {Satania.db.getExperience(User.Id.ToString())} XP `";
            embed.AddField(ExpField);

            //userid
            EmbedFieldBuilder UserIDField = new EmbedFieldBuilder();
            UserIDField.IsInline = true;
            UserIDField.Name = "User ID:";
            UserIDField.Value = User.Id.ToString();
            embed.AddField(UserIDField);

            //account creation date
            EmbedFieldBuilder DateField = new EmbedFieldBuilder();
            DateField.IsInline = true;
            DateField.Name = "Account Creation Date:";
            DateField.Value = User.CreatedAt.ToString("d");
            embed.AddField(DateField);

            //account creation date
            EmbedFieldBuilder JoinedField = new EmbedFieldBuilder();
            JoinedField.IsInline = true;
            JoinedField.Name = "Joined Server:";
            JoinedField.Value = User.JoinedAt.Value.ToString("d");
            embed.AddField(JoinedField);

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

        [Command("avatar")]
        [Summary("Shows full size image from specified user")]
        [Remarks("avatar tromodolo")]
        public async Task Avatar(SocketGuildUser User = null)
        {
            if(User == null)
            {
                var useravatar = Context.Message.Author.GetAvatarUrl(ImageFormat.Auto, 1024);
                if (useravatar.Contains(".gif"))
                {
                    var url = useravatar.Substring(0, useravatar.Length - 10);
                    await Context.Channel.SendImageEmbedAsync(url, "Avatar for user " + Context.Message.Author.Username + ":");
                }
                else
                    await Context.Channel.SendImageEmbedAsync(Context.Message.Author.GetAvatarUrl(ImageFormat.Auto, 1024), "Avatar for user " + Context.Message.Author.Username + ":");
            }
            else
            {
                var useravatar = User.GetAvatarUrl(ImageFormat.Auto, 1024);
                if (useravatar.Contains(".gif"))
                {
                    var url = useravatar.Substring(0, useravatar.Length - 10);
                    await Context.Channel.SendImageEmbedAsync(url, "Avatar for user " + User.Username + ":");
                }
                else
                    await Context.Channel.SendImageEmbedAsync(User.GetAvatarUrl(ImageFormat.Auto, 1024), "Avatar for user " + User.Username + ":");
            }
        }

    }
}

