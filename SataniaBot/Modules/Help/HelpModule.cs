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
                embedfield.Name = $"**{m.Name}**";
                embedfield.Value = string.Join(", ", m.Commands.Select(x => x.Aliases.First()));

                embed.AddField(embedfield);

            }

            await ReplyAsync("", embed:embed);

        }
    }
}
