using Discord;
using Discord.Addons.InteractiveCommands;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Drawing;

namespace SataniaBot.Modules.Social
{
    public class newSocialModule : SocialModule
    {
        [Command("bprofile")]
        [Summary("Shows specified users profile or your own if unspecified as Image")]
        [Remarks("bprofile tromodolo")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task bProfile(IGuildUser User = null)
        {
            try
            {
                var channel = Context.Channel;
                if (User == null)
                User = Context.Message.Author as IGuildUser;

            string MarriedID = Satania.db.getMarriage(User.Id.ToString());
            bool isMarried = false;
            SocketUser MarriedPerson = null;
            if (!string.IsNullOrWhiteSpace(MarriedID))
            {
                MarriedPerson = Satania._client.GetUser(Convert.ToUInt64(MarriedID));
                isMarried = true;
            }
                //level
                var userstats = Satania.db.getLevel(User.Id.ToString());

                //experience
                EmbedFieldBuilder ExpField = new EmbedFieldBuilder();

                //userid
                EmbedFieldBuilder UserIDField = new EmbedFieldBuilder();

                //account creation date
                EmbedFieldBuilder DateField = new EmbedFieldBuilder();

                //account creation date
                EmbedFieldBuilder JoinedField = new EmbedFieldBuilder();


                MagickImage profile = new MagickImage(new MagickColor("#212128"), 256, 256);
                profile.Quality = 100;
                profile.Settings.TextEncoding = Encoding.Unicode;

                // Header Background
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#313139"))
                    .Rectangle(0, 0, profile.Width, 42));

                // Header Split Line
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#36363c"))
                    .Rectangle(0, 42, profile.Width, 43));

                // Header Username
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#eeeeee"))
                    .Text(80, 28, User.Username.ToString())
                    .Font("sans-serif", FontStyleType.Normal, FontWeight.Bold, FontStretch.Normal)
                    .FontPointSize(14));
                // Header CreatedAt
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#eeeeee"))
                    .Text(80, 40, User.CreatedAt.ToString("yyyy/mm/dd HH:mm"))
                    .Font("sans-serif", FontStyleType.Normal, FontWeight.Normal, FontStretch.Normal)
                    .FontPointSize(10));


                // Body: Progress
                // BG
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#313139"))
                    .Rectangle(80, 72, 248, 50));
                // InnerProgress
                double percent = (((double)userstats.currentExp / (double)userstats.levelExp) * 166);
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#2196f3"))
                    .Rectangle(82, 70, 82+percent, 52));
                // EndBody: Progress
                // Body: Info
                List<string> roles = new List<string>();
                foreach(var role in Context.Guild.Roles)
                {
                    foreach(var UserRoles in User.RoleIds)
                    {
                        if(UserRoles == role.Id && !role.IsEveryone)
                        {
                            roles.Add(role.Name);
                        }
                    }
                }
                string rolenames = string.Empty;
                string rntemp = string.Empty;
                if (roles.Count > 0)
                {
                    foreach(var rolen in roles)
                    {
                        if (getStringWidth(rntemp) > 190)
                        {
                            rntemp = string.Empty;
                            rolenames += "\n";
                        }
                        if(rolen != roles[roles.Count-1])
                        {
                            rntemp += $"{rolen}, ";
                            rolenames += $"{rolen}, ";
                        } else
                        {
                            rntemp += $"{rolen}";
                            rolenames += $"{rolen}";
                        }
                    }
                } else
                {
                    rolenames = "No Roles";
                }
                profile.Draw(new Drawables()
                .FillColor(new MagickColor("#eeeeee"))
                .Text(20, 100, "Roles:")
                .Font("sans-serif", FontStyleType.Normal, FontWeight.Normal, FontStretch.Normal)
                .TextAlignment(TextAlignment.Left)
                .FontPointSize(12));
                profile.Draw(new Drawables()
                .FillColor(new MagickColor("#eeeeee"))
                .Text(60, 100, rolenames)
                .Font("sans-serif", FontStyleType.Normal, FontWeight.Normal, FontStretch.Normal)
                .TextAlignment(TextAlignment.Left)
                .FontPointSize(12));
                // EndBody: Info
                // Body: XP


                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#ffffff"))
                    .Text(242, 65, $"{userstats.currentExp}/{userstats.levelExp} XP")
                    .TextAlignment(TextAlignment.Right)
                    .TextAntialias(true)
                    .Font("sans-serif", FontStyleType.Normal, FontWeight.Normal, FontStretch.Normal)
                    .FontPointSize(12));
                // Body: Level
                profile.Draw(new Drawables()
                    .FillColor(new MagickColor("#ffffff"))
                    .Text(88, 65, $"Lv.{userstats.level}")
                    .TextAlignment(TextAlignment.Left)
                    .TextAntialias(true)
                    .Font("sans-serif", FontStyleType.Normal, FontWeight.Bold, FontStretch.Normal)
                    .FontPointSize(12));

                MagickImage avatar = new MagickImage();
                avatar.Read(GetImageFromURL(User.GetAvatarUrl(ImageFormat.Jpeg, 128)));
                avatar.Resize(64, 64);
                profile.Composite(avatar, 10, 10, CompositeOperator.Over);

                // Avatar Frame
                profile.Draw(new Drawables()
                    .StrokeColor(new MagickColor("#313139"))
                    .FillColor(new MagickColor(0, 0, 0, 0))
                    .StrokeWidth(2)
                    .Rectangle(10, 10, 74, 74));

                Stream file = new MemoryStream();
                profile.Write(file, MagickFormat.Jpg);
                if(file.Length > 0 && file.CanRead)
                {
                    Console.WriteLine($"Attempt to send Profile Image: {channel.Id}");
                    file.Position = 0;
                    var r = await (Satania._client.GetChannel(channel.Id) as IMessageChannel).SendFileAsync(file, $"user-{User.Id}.{profile.Format.ToString().ToLower()}");
                    Console.WriteLine($"Sent Profile Image: {r.Channel}");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
        private static Stream GetImageFromURL(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponseAsync().Result;
            Stream stream = httpWebReponse.GetResponseStream();
            return stream;
        }
        private static float getStringWidth(string str, string font = "sans-serif", int fontsize = 12)
        {
            Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            return gr.MeasureString(str, new Font(font, fontsize)).Width;
        }
    }

}
