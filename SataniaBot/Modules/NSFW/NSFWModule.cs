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
        [Summary("Gets image from safebooru and sends image. Gives random image if no tag.")]
        [Remarks("s?safebooru kizuna_ai")]
        public async Task Safebooru([Remainder]string tags = null)
        {
            if (tags == null)
                tags = "rating:safe";

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
        [Summary("Gets image from gelbooru and sends image. Add +rating:explicit to the end for NSFW only. Gives random NSFW image if no tag.")]
        [Remarks("s?gelbooru kizuna_ai+rating:explicit")]
        public async Task Gelbooru([Remainder]string tags = null)
        {
            if (tags == null)
                tags = "rating:explicit";

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
        [Summary("Gets image from rule34 and sends image. Add +rating:explicit to the end for NSFW only. Gives random NSFW image if no tag.")]
        [Remarks("s?rule34 kizuna_ai+rating:explicit")]
        public async Task Rule34([Remainder]string tags = null)
        {
            if (tags == null)
                tags = "rating:explicit";

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
        [Summary("Gets image from konachan and sends image. Gives random image if no tag.")]
        [Remarks("s?konachan dark souls")]
        public async Task Konachan([Remainder]string tags = null)
        {
            if (tags == null)
                tags = "rating:safe";

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
        [Summary("Gets image from yandere and sends image. Add +rating:explicit to the end for NSFW only. Gives random NSFW image if no tag.")]
        [Remarks("s?yandere kizuna_ai+rating:explicit")]
        public async Task Yandere([Remainder]string tags = null)
        {
            if (tags == null)
                tags = "rating:explicit";

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
        [Summary("Gets image from e621 and sends image. Add +rating:explicit to the end for NSFW only. Gives random NSFW image if no tag.")]
        [Remarks("s?e621 dragon+rating:explicit")]
        public async Task E621([Remainder]string tags = null)
        {
            if (tags == null)
                tags = "rating:explicit";

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

