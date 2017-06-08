using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SataniaBot.Services.EmbedExtensions;
using Satania;

namespace SataniaBot.Modules
{
    [Name("Utility")]
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        [Command("roles")]
        [Summary("Shows list of self-assignable roles available in a server.")]
        [Remarks("s?roles")]
        public async Task roles()
        {
            var results = Satania.db.getServerRole(Context.Guild.Id.ToString());
            await ReplyAsync(results);

        }

        [Command("setrole")]
        [Summary("Adds a role to self-assignable roles")]
        [Remarks("s?setrole civilian")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task setrole(IRole role)
        {
            try         //Basically tries adding role to user to check if it has permissions to do it. Because I'm bad at finding out how otherwise
            {
                await (Context.Message.Author as IGuildUser).AddRoleAsync(role);
                await (Context.Message.Author as IGuildUser).RemoveRoleAsync(role);
                Satania.db.addRole(role, Context.Guild.Id.ToString());
                await Context.Channel.SendConfirmAsync("Role was set as a self-assignable role.");
            }
            catch (Exception e)
            {
                await Context.Channel.SendErrorAsync("Role needs to be below Erin in role list or it's already added. :(");
            }
        }

        [Command("unsetrole")]
        [Summary("Removes a role from self-assignable")]
        [Remarks("s?unsetrole civilian")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task unsetRole(IRole role)
        {
            try         //Basically tries adding role to user to check if it has permissions to do it. Because I'm bad at finding out how otherwise
            {
                await (Context.Message.Author as IGuildUser).AddRoleAsync(role);
                await (Context.Message.Author as IGuildUser).RemoveRoleAsync(role);
                Satania.db.removeRole(role, Context.Guild.Id.ToString());
                await Context.Channel.SendConfirmAsync("Role was removed as a self-assignable role.");
            }
            catch (Exception e)
            {
                await Context.Channel.SendErrorAsync("Role needs to be below Erin in role list or the role wasn't added. :(");
            }
        }

        [Command("giverole")]
        [Summary("Gives you a role from one of the self-assignable ones")]
        [Remarks("s?giverole civilian")]
        public async Task giveRole(IRole role)
        {
            if (Satania.db.checkServerRole(Context.Guild.Id.ToString(), role.Id.ToString()))
            {
                await (Context.Message.Author as IGuildUser).AddRoleAsync(role);
                await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}, you now have the role: {role.Name}");
            }
            else
            {
                await Context.Channel.SendErrorAsync("That role is not one of the self-assignable roles.");
            }
        }

        [Command("takeRole")]
        [Summary("Removes a role you have from one of the self-assignable ones")]
        [Remarks("s?takeRole civilian")]
        public async Task takeRole(IRole role)
        {
            if (Satania.db.checkServerRole(Context.Guild.Id.ToString(), role.Id.ToString()))
            {
                await (Context.Message.Author as IGuildUser).RemoveRoleAsync(role);
                await Context.Channel.SendConfirmAsync($"{Context.Message.Author.Mention}, you no longer have the role : {role.Name}");     //This will run whether the role was existing to begin with or not, I'll come back to this later
            }
            else
                return;
        }
    }
}
