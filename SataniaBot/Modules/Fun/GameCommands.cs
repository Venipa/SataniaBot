using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using SataniaBot.Services.EmbedExtensions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SataniaBot.Modules
{
    [Name("Games")]
    public class GameModules : InteractiveModuleBase<SocketCommandContext>
    {
        [Command("CoinFlip")]
        public async Task Flip()
        {

            Random flip = new Random();
            var coin = (flip.Next(1, 100));
            string CoinSide = "";
            if (coin <= 50)
            {
                CoinSide = "Heads";
            }
            else if (coin >= 51)
            {
                CoinSide = "Tails";
            }

            await Context.Channel.SendConfirmAsync(CoinSide);

        }

        
    }
}

