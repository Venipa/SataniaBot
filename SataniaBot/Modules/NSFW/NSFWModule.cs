using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using SataniaBot.Services.EmbedExtensions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Satania.Services;

namespace SataniaBot.Modules
{
    [Name("NSFW")]
    public class NSFWModule : InteractiveModuleBase<SocketCommandContext>
    {
        [Command("danbooru")]
        public async Task Danbooru([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                await Context.Channel.SendImageEmbedAsync(SearchHelper.GetDanbooruImageLink(tags).Result);
            }
            else
            {
                return;
            }
        }

        [Command("gelbooru")]
        public async Task Gelbooru([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                await Context.Channel.SendImageEmbedAsync(SearchHelper.GetGelbooruImageLink(tags).Result);
            }
            else
            {
                return;
            }
        }

        [Command("safebooru")]
        public async Task Safebooru([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                await Context.Channel.SendImageEmbedAsync(SearchHelper.GetSafebooruImageLink(tags).Result);
            }
            else
            {
                return;
            }
        }

        [Command("e621")]
        public async Task E621([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                await Context.Channel.SendImageEmbedAsync(SearchHelper.GetE621ImageLink(tags).Result);
            }
            else
            {
                return;
            }
        }

        [Command("rule34")]
        public async Task Rule34([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                await Context.Channel.SendImageEmbedAsync(SearchHelper.GetRule34ImageLink(tags).Result);
            }
            else
            {
                return;
            }
        }
    }
}

