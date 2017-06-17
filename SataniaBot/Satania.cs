using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using SataniaBot.Services;
using System.Linq;
using System.Threading;
using System.Drawing;
using Console = Colorful.Console;


namespace SataniaBot
{
    public enum botNameId
    {
        Satania = 1,
        Erin = 2
    }
    public class Satania
    {
        public static void Main(string[] args)
        => new Satania().Start().GetAwaiter().GetResult();

        public static DatabaseHandler db = new DatabaseHandler();
        public static DiscordSocketClient _client;
        public static CommandHandler _commands = new CommandHandler();

        public async Task Start()
        {
            Configuration.EnsureExists();                    // Ensure the configuration file has been created.
                                                             // Create a new instance of DiscordSocketClient.
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,              // Specify console verbose information level.
                AlwaysDownloadUsers = true,                  // Start the cache off with updated information.
                MessageCacheSize = 1000                    // Tell discord.net how long to store messages (per channel).
            });
            _client.Log += (l)                               // Register the console log event.
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.Message ?? l.Message}", System.Drawing.Color.OrangeRed));
            

            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token);
            await _client.StartAsync();

            await _commands.Install(_client);

            _client.JoinedGuild += _client_JoinedGuild;
            _client.UserJoined += _client_JoinedUser;

            await Task.Delay(5000);

            try
            {
                foreach (SocketGuild guild in _client.Guilds)
                {
                    db.addServerAsync(guild);
                    foreach(SocketUser user in guild.Users)
                    {
                        db.addUser(user, false);
                    }
                }
<<<<<<< HEAD
            } catch(Exception ex)
            {
                Console.WriteLine(ex, System.Drawing.Color.OrangeRed);
=======
                else
                {
                    db.updateServer(guild);
                }
>>>>>>> refs/remotes/Tromodolo/master
            }

            await Task.Delay(2000);
            var guildcount = _client.Guilds.Count;
            var channelcount = _client.Guilds.SelectMany(x => x.Channels).Count();
            var usercount = _client.Guilds.SelectMany(x => x.Users).Count();
            db.updateWebStatsAsync(guildcount, channelcount, usercount);
            await Task.Delay(-1);                            // Prevent the console window from closing.
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            db.addServerAsync(arg);
            foreach (SocketUser user in arg.Users)
            {
                db.addUser(user, false);
            }
        }
        private async Task _client_JoinedUser(SocketGuildUser arg)
        {
            db.addUser(arg, false);
        }
    }
}