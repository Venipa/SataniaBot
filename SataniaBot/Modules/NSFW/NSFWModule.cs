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
using static Satania.Services.SearchHelper;

namespace SataniaBot.Modules
{
    [Name("NSFW")]
    public class NSFWModule : InteractiveModuleBase<SocketCommandContext>
    {
        [Command("safebooru")]
        public async Task Safebooru([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                string result = PictureSearch(tags, WebsiteType.Safebooru).Result;
                if (string.IsNullOrWhiteSpace(result))
                    await Context.Channel.SendErrorAsync("No results found.");
                else
                    await Context.Channel.SendImageEmbedAsync(result);
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
                string result = PictureSearch(tags, WebsiteType.Gelbooru).Result;
                if (string.IsNullOrWhiteSpace(result))
                    await Context.Channel.SendErrorAsync("No results found.");
                else
                    await Context.Channel.SendImageEmbedAsync(result);
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
                string result = PictureSearch(tags, WebsiteType.Rule34).Result;
                if (string.IsNullOrWhiteSpace(result))
                    await Context.Channel.SendErrorAsync("No results found.");
                else
                    await Context.Channel.SendImageEmbedAsync(result);
            }
            else
            {
                return;
            }
        }

        [Command("konachan")]
        public async Task Konachan([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                string result = PictureSearch(tags, WebsiteType.Konachan).Result;
                if (string.IsNullOrWhiteSpace(result))
                    await Context.Channel.SendErrorAsync("No results found.");
                else
                    await Context.Channel.SendImageEmbedAsync(result);
            }
            else
            {
                return;
            }
        }

        [Command("yandere")]
        public async Task Yandere([Remainder]string tags)
        {
            if (Satania.db.checkNsfw(Context.Channel.Id.ToString()))
            {
                string result = PictureSearch(tags, WebsiteType.Yandere).Result;
                if (string.IsNullOrWhiteSpace(result))
                    await Context.Channel.SendErrorAsync("No results found.");
                else
                    await Context.Channel.SendImageEmbedAsync(result);
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
                string result = GetE621ImageLink(tags).Result;
                if (string.IsNullOrWhiteSpace(result))
                    await Context.Channel.SendErrorAsync("No results found.");
                else
                    await Context.Channel.SendImageEmbedAsync(result);
            }
            else
            {
                return;
            }
        }
    }
}

