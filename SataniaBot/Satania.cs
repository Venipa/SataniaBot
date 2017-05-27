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

            await Task.Delay(5000);
            var guildcount = _client.Guilds.Count;
            var channelcount = _client.Guilds.SelectMany(x => x.Channels).Count();
            var usercount = _client.Guilds.SelectMany(x => x.Users).Count();

            db.updateWebStats(guildcount, channelcount, usercount);

            Console.WriteLine("Connected to Discord Chat Service.\nServers: " + guildcount + " Channels: " + channelcount + " Users: " + usercount, System.Drawing.Color.LawnGreen);


            foreach(SocketGuild guild in _client.Guilds)
            {
                if(db.getPrefix(guild.Id.ToString()) == "")             //This is to make sure that a server is actually in the database on boot. 
                {                                                       //If a server isn't in the database, the bot won't respond to commands.
                    db.addServer(guild);
                }
            }

            await Task.Delay(-1);                            // Prevent the console window from closing.
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            db.addServer(arg);
        }
    }
}