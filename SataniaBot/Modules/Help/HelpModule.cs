using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SataniaBot.Modules.Help
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {

        [Command("commands")]
        [Summary("Lists all commands")]
        [Remarks("commands")]
        public async Task Commands()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = new Color(114, 137, 218);

            EmbedAuthorBuilder Author = new EmbedAuthorBuilder();
            Author.Name = string.Format("List of commands for {0}:", Satania._client.CurrentUser.Username);
            Author.IconUrl = Satania._client.CurrentUser.GetAvatarUrl();

            embed.Author = Author;

            foreach (var m in Satania._commands._cmds.Modules)
            {
                EmbedFieldBuilder embedfield = new EmbedFieldBuilder();
                embedfield.IsInline = false;
                embedfield.Name = $"**{m.Name}**";
                embedfield.Value = string.Join(", ", m.Commands.Select(x => x.Aliases.First()));

                embed.AddField(embedfield);

            }

            await ReplyAsync("", embed:embed);

        }

        [Command("help")]
        [Summary("Get help for a specific command")]
        [Remarks("help setname")]
        public async Task Help(string commandname)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = new Color(114, 137, 218);

            var commandinfo = Satania._commands._cmds.Commands.Where(s => s.Name == commandname).First();

            //command name
            EmbedFieldBuilder nameField = new EmbedFieldBuilder();
            nameField.IsInline = true;
            nameField.Name = "Name";
            nameField.Value = "`#" + commandinfo.Name + "`";
            embed.AddField(nameField);

            //Command Usage
            EmbedFieldBuilder usageField = new EmbedFieldBuilder();
            usageField.IsInline = true;
            usageField.Name = "Usage";
            usageField.Value = commandinfo.Summary;
            embed.AddField(usageField);

            //Command Parameters
            EmbedFieldBuilder parameterField = new EmbedFieldBuilder();
            parameterField.IsInline = true;
            parameterField.Name = "Parameters";
            string parameters = commandinfo.Parameters.Count > 0 ? String.Join(", ", commandinfo.Parameters.Select(x => x.Name)) : "No Parameters";
            parameterField.Value = $"` {parameters} `";              //Message won't send if no parameters because empty, so this just makes it send something at all
            embed.AddField(parameterField);
            //Command Example
            EmbedFieldBuilder exampleField = new EmbedFieldBuilder();
            exampleField.IsInline = true;
            exampleField.Name = "Example";
            exampleField.Value = "`" + Services.CommandHandler.serverPrefix + commandinfo.Remarks + "`";
            embed.AddField(exampleField);

            await ReplyAsync("", embed:embed);
        }
    }
}
