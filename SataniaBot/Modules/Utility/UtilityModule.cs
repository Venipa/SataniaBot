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
            if(results.Length > 1)
            {
                await Context.Channel.SendColouredEmbedAsync("The list of roles available in this server:", results, new Color());
            }
            else
            {
                await Context.Channel.SendColouredEmbedAsync("The list of roles available in this server:", "There are no available roles", new Color());
            }
        }

        [Command("setrole")]
        [Summary("Adds a role to self-assignable roles. Needs manage messages permissions.")]
        [Remarks("s?setrole civilian")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task setrole(IRole role)
        {
            try 
            {
                int authorPos = (Context.Message.Author as SocketGuildUser).Hierarchy;
                int rolePos = role.Position;
                int botPos = Context.Guild.CurrentUser.Hierarchy;

                if (botPos <= rolePos)
                {
                    await Context.Channel.SendErrorAsync("The role needs to be below the bot in role-list.");
                }
                else if (authorPos <= rolePos)
                {
                    await Context.Channel.SendErrorAsync("You can't add a role that is equal to your own or higher to the self-assign.");
                }
                else
                {
                    Satania.db.addRole(role, Context.Guild.Id.ToString());
                    await Context.Channel.SendConfirmAsync("Role was set as a self-assignable role.");
                }
                    
            }
            catch
            {
                await Context.Channel.SendErrorAsync("The role is already added to self-assignable. :( ");
            }
        }

        [Command("unsetrole")]
        [Summary("Removes a role from self-assignable. Needs manage messages permissions.")]
        [Remarks("s?unsetrole civilian")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task unsetRole(IRole role)
        {
            try     
            {
                Satania.db.removeRole(role, Context.Guild.Id.ToString());
                await Context.Channel.SendConfirmAsync("Role was removed as a self-assignable role.");
            }
            catch
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
