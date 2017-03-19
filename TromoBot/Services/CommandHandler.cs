﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TromoBot
{
    /// <summary> Detect whether a message is a command, then execute it. </summary>
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;                                                 // Save an instance of the discord client.
            _cmds = new CommandService();                                // Create a new instance of the commandservice.                              

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());    // Load all modules from the assembly.

            _client.MessageReceived += HandleCommand;                    // Register the messagereceived event to handle commands.
        }

        private async Task HandleCommand(SocketMessage s)
        {
            var serverPrefix = TromoBot.db.getPrefix((s.Channel as IGuildChannel)?.Guild.Id.ToString());
            var msg = s as SocketUserMessage;
            if (msg == null)                                          // Check if the received message is from a user.
                return;
            if(serverPrefix == null)
                return;

            var context = new SocketCommandContext(_client, msg);     // Create a new command context.

            int argPos = 0;                                           // Check if the message has either a string or mention prefix.
            if (msg.HasStringPrefix(serverPrefix, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {                                                         // Try and execute a command with the given context.
                var result = await _cmds.ExecuteAsync(context, argPos);

                //if (!result.IsSuccess)                                // If execution failed, reply with the error message.
                //   Console.WriteLine(result.ToString());
                //      Commented out because otherwise bot either spams console or chat with error message when it doesnt find a command, unneeded
            }
        }
    }
}