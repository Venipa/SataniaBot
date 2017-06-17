using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Drawing;
using Cleverbot.Net;
using Console = Colorful.Console;

namespace SataniaBot.Services
{
    /// <summary> Detect whether a message is a command, then execute it. </summary>
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        public CommandService _cmds;
        CleverbotSession cleverbot = new CleverbotSession(Configuration.Load().CleverbotApi, false, Configuration.Load().EnableCleverbot);

        public static string serverPrefix { get; set; }
        private static string defaultPrefix = "s?";
        private static string defaultMoneySuffix = "Yen";
        public static string serverMoneySuffix { get; set; }
        public async Task Install(DiscordSocketClient c)
        {
            _client = c;                                                 // Save an instance of the discord client.
            _cmds = new CommandService();                                // Create a new instance of the commandservice.                              

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());    // Load all modules from the assembly.

            _client.MessageReceived += HandleCommand;                    // Register the messagereceived event to handle commands.
        }

        private async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)                                          // Check if the received message is from a user.
                return;
            if(s.Author.IsBot)                                        // Check if the author of the message is a bot.
                return;

            IGuild guild = (s.Channel as IGuildChannel)?.Guild;
            serverPrefix = Satania.db.getPrefix(guild.Id.ToString());
            serverMoneySuffix = Satania.db.getMoneySuffix(guild.Id.ToString());
            serverMoneySuffix = serverMoneySuffix != "" ? serverMoneySuffix : defaultMoneySuffix;
            Random xpRandom = new Random();
            DateTime? nowTime = DateTime.Now;
            DateTime? oldTime = Satania.db.getTimer(s.Author.Id.ToString());
            if (oldTime == null)
            {
                Satania.db.updateTimer(s.Author.Id.ToString());
            }
            else if ((nowTime - oldTime).Value.TotalSeconds > 60)
            {
                Satania.db.incrementExperience(s, xpRandom.Next(10, 15));
                Satania.db.updateTimer(s.Author.Id.ToString());
            }

            var context = new SocketCommandContext(_client, msg);     // Create a new command context.

            int argPos = 0;
            // Check if the message has either a string or mention prefix.
            IResult result = null;
            if (msg.HasStringPrefix(serverPrefix, ref argPos) ||
                msg.HasStringPrefix(defaultPrefix, ref argPos))
            {                                                         // Try and execute a command with the given context.
                Satania.db.addUser(msg.Author, false, false);
                foreach(var users in msg.MentionedUsers)
                {
                    Satania.db.addUser(msg.Author);
                }
                result = await _cmds.ExecuteAsync(context, argPos);
                if (result.IsSuccess)
                {
                    Satania.db.incrementCommands();
                    Console.WriteLine("\nCommand " + s.Content + " used by user " + s.Author + ":", System.Drawing.Color.Cyan);
                    Console.WriteLine("In Server: " + guild.Name + $" ({guild.Id})", System.Drawing.Color.Cyan);
                    Console.WriteLine("In Channel: " + s.Channel.Name + $" ({s.Channel.Id})", System.Drawing.Color.Cyan);
                }
                //if (!result.IsSuccess)                                // If execution failed, reply with the error message.
                //   Console.WriteLine(result.ToString());
                //      Commented out because otherwise bot either spams console or chat with error message when it doesnt find a command, unneeded
            }
            else if(msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var test = msg.Content.Replace($"<@{_client.CurrentUser.Id}>", "");
                await context.Channel.TriggerTypingAsync();
                var response = await cleverbot.GetResponseAsync(test);
                await msg.Channel.SendMessageAsync(response.Response);
            }

            // Same If like above, but without repeating the code, Logs all commands only if it has been set to a channel and if the Command Execution is successfully.
            // Logs: User(Silent Mention), Command with Parameters and Channel
            if((msg.HasStringPrefix(serverPrefix, ref argPos)
                || msg.HasStringPrefix(defaultPrefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                && result != null && result.IsSuccess)
            {
                ulong logId = Satania.db.getLog(guild.Id.ToString());
                if (logId != 0)
                {
                    EmbedBuilder logMsg = new EmbedBuilder();
                    logMsg.Color = new Discord.Color(33, 150, 243);
                    EmbedFieldBuilder logAuthor = new EmbedFieldBuilder();
                    logAuthor.IsInline = true;
                    logAuthor.Name = "Username";
                    logAuthor.Value = $"{s.Author.Mention}";
                    logMsg.AddField(logAuthor);
                    EmbedFieldBuilder logChannel = new EmbedFieldBuilder();
                    logChannel.IsInline = true;
                    logChannel.Name = "Channel";
                    logChannel.Value = $"#{s.Channel.Name} ({s.Channel.Id})";
                    logMsg.AddField(logChannel);
                    EmbedFieldBuilder logCommand = new EmbedFieldBuilder();
                    logCommand.IsInline = false;
                    logCommand.Name = "Command";
                    logCommand.Value = s.Content;
                    logMsg.AddField(logCommand);
                    var logresult = await (_client.GetChannel(logId) as IMessageChannel).SendMessageAsync("", false, embed: logMsg);
                }
            }
        }
    }
}