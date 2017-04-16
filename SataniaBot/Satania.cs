using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using SataniaBot.Services;
using System.Linq;
using System.Threading;


namespace SataniaBot
{
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
                LogLevel = LogSeverity.Verbose,              // Specify console verbose information level.
                AlwaysDownloadUsers = true,                  // Start the cache off with updated information.
                MessageCacheSize = 1000                      // Tell discord.net how long to store messages (per channel).
            });

            _client.Log += (l)                               // Register the console log event.
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));

            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token);
            await _client.StartAsync();

            await _commands.Install(_client);

            _client.JoinedGuild += _client_JoinedGuild;

            await Task.Delay(5000);

            db.updateWebStats(_client.Guilds.Count, _client.Guilds.SelectMany(x => x.Channels).Count(), _client.Guilds.SelectMany(x => x.Users).Count());

            await Task.Delay(-1);                            // Prevent the console window from closing.
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            db.addServer(arg);
        }
    }
}