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
        public async Task Commands()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = new Color(114, 137, 218);

            foreach (var m in Satania._commands._cmds.Modules)
            {
                EmbedFieldBuilder embedfield = new EmbedFieldBuilder();
                embedfield.IsInline = true;
                embedfield.Name = $"**{m.Name}**";
                embedfield.Value = string.Join(", ", m.Commands.Select(x => x.Aliases.First()));

                embed.AddField(embedfield);

            }

            await ReplyAsync("", embed:embed);

        }

        [Command("help")]
        public async Task Help(string commandname)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = new Color(114, 137, 218);

            var commandinfo = Satania._commands._cmds.Commands.Where(s => s.Name == commandname).First();

            //command name
            EmbedFieldBuilder nameField = new EmbedFieldBuilder();
            nameField.IsInline = true;
            nameField.Name = "Name";
            nameField.Value = "`" + Satania.db.getPrefix(Context.Guild.Id.ToString()) + commandinfo.Name + "`";
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
            parameterField.Value = String.Join(", ", commandinfo.Parameters.Select(x => x.Name));
            embed.AddField(parameterField);

            //Command Example
            EmbedFieldBuilder exampleField = new EmbedFieldBuilder();
            exampleField.IsInline = true;
            exampleField.Name = "Example";
            exampleField.Value = "`" + Satania.db.getPrefix(Context.Guild.Id.ToString()) + commandinfo.Remarks + "`";
            embed.AddField(exampleField);

            await ReplyAsync("", embed:embed);
        }
    }
}
