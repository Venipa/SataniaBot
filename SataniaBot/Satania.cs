﻿using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using SataniaBot.Services;

namespace SataniaBot
{
    public class Satania
    {
        public static void Main(string[] args)
        => new Satania().Start().GetAwaiter().GetResult();

        public static DatabaseHandler db = new DatabaseHandler();
        public static DiscordSocketClient _client;
        private CommandHandler _commands;

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

            _commands = new CommandHandler();                // Initialize the command handler service
            await _commands.Install(_client);

            _client.JoinedGuild += _client_JoinedGuild;

            await Task.Delay(-1);                            // Prevent the console window from closing.
        }

        private async Task _client_JoinedGuild(SocketGuild arg)
        {
            db.addServer(arg);
            await arg.DownloadUsersAsync();             //only reason i have this here is because the task threw a fit because i didnt have any return value
        }
    }
}