using System;
using MySql.Data.MySqlClient;
using Discord.WebSocket;
using System.Linq;
using Discord;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SataniaBot.Database.Tables;
using System.Threading.Tasks;

namespace SataniaBot.Services
{
    public class DatabaseHandler
    {

        MySqlContext context;
        public DatabaseHandler()
        {
            context = new MySqlContext();
            context.Database.EnsureCreated();
        }

        public string getPrefix(string guildid)
        {
            try
            {
                var res = context.serversettings.FirstOrDefault(x => x.serverid == guildid);
                if (res != null)
                {
                    return res.commandprefix;
                }
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }
        public string getMoneySuffix(string guildid)
        {
            try
            {
                var res = context.serversettings.FirstOrDefault(x => x.serverid == guildid);
                if (res != null)
                {
                    return res.currencyname;
                }
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

        public ulong getLog(string guildid)
        {
            try
            {
                var res = context.logchannels.FirstOrDefault(x => x.serverid == guildid);
                if (res != null)
                {
                    return Convert.ToUInt64(res.channelid);
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public void updatePrefix(SocketGuild s, string prefix)
        {
            try
            {
                var res = context.serversettings.FirstOrDefault(x => x.serverid == s.Id.ToString());
                res.commandprefix = prefix;
                context.serversettings.Update(res);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        public void updateMoneySuffix(SocketGuild s, string suffix)
        {
            try
            {
                var res = context.serversettings.FirstOrDefault(x => x.serverid == s.Id.ToString());
                res.currencyname = suffix;
                context.serversettings.Update(res);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }


        public void addServerAsync(SocketGuild s)
        {
            try
            {
                var res = context.serversettings.FirstOrDefault(x => x.serverid == s.Id.ToString());
                if (res == null)
                {
                    context.serversettings.Add(new serversettings
                    {
                        serverid = s.DefaultChannel.Id.ToString(),
                        commandprefix = "s?",
                        currencyname = "Yen"
                    });
                    context.SaveChanges();
                }



                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string getMarriage(string userid)
        {
            try
            {

                var res = context.usermarriages.FirstOrDefault(x => x.userid == userid);
                if (res != null)
                {
                    return res.marriedid;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void addMarriage(string person1, string person2)
        {
            if (person1 == person2)
                return;

            context.usermarriages.Add(new usermarriages
            {
                userid = person1,
                marriedid = person2
            });
            context.usermarriages.Add(new usermarriages
            {
                userid = person2,
                marriedid = person1
            });
            context.SaveChanges();
        }

        public void removeMarriage(string person1, string person2)
        {
            var p1 = context.usermarriages.FirstOrDefault(x => x.userid == person1);
            var p2 = context.usermarriages.FirstOrDefault(x => x.userid == person2);
            if (p1 != null && p2 != null)
            {
                context.usermarriages.Remove(p1);
                context.usermarriages.Remove(p2);
                context.SaveChanges();
            }
        }

        public void updateWebStatsAsync(int servernum, int channelnum, int usernum)
        {
            var res = context.usagestats.FirstOrDefault(x => x.key == Convert.ToInt32(botNameId.Satania));
            if (res != null)
            {
                res.servercount = servernum;
                res.channelcount = channelnum;
                res.usercount = usernum;
                context.usagestats.Update(res);
            }
            else
            {
                res = new usagestats();
                res.servercount = servernum;
                res.channelcount = channelnum;
                res.usercount = usernum;
                res.commandusage = 0;
                context.usagestats.Add(res);
            }
            context.SaveChanges();
        }

        public void incrementCommands()
        {
            var res = context.usagestats.FirstOrDefault(x => x.key == Convert.ToInt32(botNameId.Satania));
            if (res != null)
            {
                res.commandusage++;
                context.usagestats.Update(res);
                context.SaveChanges();
            }
        }

        public void addNsfwChannel(string channelid)
        {
            nsfwchannels res = new nsfwchannels()
            {
                channelid = channelid
            };
            context.nsfwchannels.Add(res);
            context.SaveChanges();
        }

        public void removeNsfwChannel(string channelid)
        {
            var res = context.nsfwchannels.FirstOrDefault(x => x.channelid == channelid);
            if (res != null)
            {
                context.nsfwchannels.Remove(res);
                context.SaveChanges();
            }
        }

        public bool checkNsfw(string channelid)
        {
            try
            {
                var res = context.nsfwchannels.FirstOrDefault(x => x.channelid == channelid);

                if (res != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public void addLogChannel(string channelid, string serverid)
        {
            logchannels res = new logchannels()
            {
                serverid = serverid,
                channelid = channelid
            };
            context.logchannels.Add(res);
            context.SaveChanges();
        }

        public void removeLogChannel(string serverid)
        {
            var res = context.logchannels.FirstOrDefault(x => x.serverid == serverid);
            if (res != null)
            {
                context.logchannels.Remove(res);
                context.SaveChanges();
            }
        }

        public bool checkLog(string serverid)
        {
            try
            {
                var res = context.logchannels.FirstOrDefault(x => x.serverid == serverid);

                if (res != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public DateTime? getTimer(string userid)
        {

            var res = context.experiencetimers.FirstOrDefault(x => x.userid == userid);
            if (res != null)
            {
                return Convert.ToDateTime(res.lastmessage);
            }
            return null;
        }

        public void updateTimer(string userid)
        {
            DateTime Now = DateTime.Now;
            if (getTimer(userid) == null)
            {
                context.experiencetimers.Add(new experiencetimers()
                {
                    userid = userid,
                    lastmessage = Now
                });
            }
            else
            {
                var res = context.experiencetimers.FirstOrDefault(x => x.userid == userid);
                if (res != null)
                {
                    res.lastmessage = Now;
                }
                context.experiencetimers.Update(res);
            }
            context.SaveChanges();
        }

        public int getRep(string userid)
        {
            var res = context.userrep.FirstOrDefault(x => x.userid == userid);
            return res.reps;
        }
        public string setRep(SocketUser user, string repid)
        {
            try
            {
                var res = context.userreptimers.FirstOrDefault(x => x.userid == user.Id.ToString());
                var r = context.userrep.FirstOrDefault(x => x.userid == repid);
                if (res != null && r != null)
                {
                    TimeSpan remainingTime = (DateTime.Now - res.lastrep);
                    if (remainingTime.TotalHours > 24)
                    {
                        r.reps += 1;
                        context.userrep.Update(r);
                        res.lastrep = DateTime.Now;
                        context.userreptimers.Update(res);
                        context.SaveChanges();
                        return null;
                    }
                    else
                    {
                        var remaining = TimeSpan.FromHours(24) - remainingTime;
                        return $"**{remaining.Hours} Hour{(remaining.Hours > 1 ? "s" : "")}**, **{remaining.Minutes} Minute{(remaining.Minutes > 1 ? "s" : "")}** and **{remaining.Seconds} Second{(remaining.Seconds > 1 ? "s" : "")}** until you can Rep someone again.";
                    }
                }
                else
                {
                    if (r != null)
                    {
                        r.reps += 1;
                        context.userrep.Update(r);
                    }
                    else
                    {
                        context.userrep.Add(new userrep()
                        {
                            userid = repid,
                            reps = 1
                        });
                    }

                    if (res != null)
                    {
                        res.lastrep = DateTime.Now;
                        context.userreptimers.Update(res);
                    }
                    else
                    {
                        context.userreptimers.Add(new userreptimers()
                        {
                            userid = user.Id.ToString(),
                            lastrep = DateTime.Now
                        });
                    }
                    context.SaveChanges();
                    return null;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return "Something wrong happened";
            }

        }
        public string getMoney(SocketUser user)
        {
            try
            {
                var res = context.usermoney.FirstOrDefault(x => x.userid == user.Id.ToString());
                if (res != null)
                {
                    return res.money.ToString();
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public string addMoney(SocketUser user, uint moneygain, bool addTimeLimit = true)
        {
            var res = context.usermoney.FirstOrDefault(x => x.userid == user.Id.ToString());
            var restime = context.userdaily.FirstOrDefault(x => x.userid == user.Id.ToString());
            if (res != null && restime != null)
            {
                if(!addTimeLimit)
                {
                    res.money += moneygain;
                    context.usermoney.Update(res);
                    return null;
                }
                TimeSpan remainingTime = (DateTime.Now - restime.lastdaily);
                if (remainingTime.TotalHours > 24)
                {
                    res.money += moneygain;
                    restime.lastdaily = DateTime.Now;
                    context.usermoney.Update(res);
                    context.userdaily.Update(restime);
                    context.SaveChanges();
                    return null;
                }
                else
                {
                    var remaining = TimeSpan.FromHours(24) - remainingTime;
                    return $"**{remaining.Hours} Hour{(remaining.Hours > 1 ? "s" : "")}**, **{remaining.Minutes} Minute{(remaining.Minutes > 1 ? "s" : "")}** and **{remaining.Seconds} Second{(remaining.Seconds > 1 ? "s" : "")}** until you can get your Daily again.";
                }
            }
            else
            {
                if (res != null)
                {
                    res.money += moneygain;
                    context.usermoney.Update(res);
                }
                else
                {
                    context.usermoney.Add(new usermoney()
                    {
                        money = moneygain,
                        userid = user.Id.ToString()
                    });
                }
                if (restime != null)
                {
                    restime.lastdaily = DateTime.Now;
                    context.userdaily.Update(restime);
                }
                else
                {
                    context.userdaily.Add(new userdailytimer()
                    {
                        userid = user.Id.ToString(),
                        lastdaily = DateTime.Now
                    });
                }
            }
            context.SaveChanges();
            return null;
        }
        public bool transferMoney(SocketUser user = null, SocketUser destUser = null, uint amount = 0)
        {
            try
            {
                var user1 = context.usermoney.FirstOrDefault(x => x.userid == user.Id.ToString());
                var user2 = context.usermoney.FirstOrDefault(x => x.userid == destUser.Id.ToString());
                if (user == null || destUser == null || amount <= 0)
                {
                    return false;
                }
                if (user1.money < amount)
                {
                    return false;
                }
                else
                {
                    user1.money -= amount;
                }
                context.usermoney.Update(user1);
                context.SaveChanges();
                if (user2 != null)
                {
                    user2.money += amount;
                    context.usermoney.Update(user2);
                } else
                {
                    context.usermoney.Add(new usermoney()
                    {
                        userid = destUser.Id.ToString(),
                        money = amount
                    });
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }
        public void incrementExperience(SocketMessage msg, int experiencegain)
        {
            var user = msg.Author;
            var res = context.userexperience.FirstOrDefault(x => x.userid == user.Id.ToString());
            if (res != null)
            {
                res.experience += experiencegain;
                var current = getLevel(user.Id.ToString());
                if ((current.currentExp + experiencegain) > current.levelExp)
                {
                    msg.Channel.SendMessageAsync($"{msg.Author.Mention} had leveled to Lv.{++current.level}");
                }
            }
            else
            {
                context.userexperience.Add(new userexperience()
                {
                    experience = experiencegain,
                    userid = user.Id.ToString()
                });
            }
            context.SaveChanges();
            return;
        }

        public int? getExperience(string userid)
        {
            var res = context.userexperience.FirstOrDefault(x => x.userid == userid);
            if (res != null)
            {
                return res.experience as int?;
            }
            return 0;
        }

        public (int? level, int? currentExp, int? levelExp) getLevel(string userid)
        {
            var res = context.userexperience.FirstOrDefault(x => x.userid == userid);
            if (res != null)
            {
                var totalExp = res.experience;

                int? currentExp = 0, levelExp = 0, level = 1;
                int? totalExperience = (totalExp as int?);

                int loop = 1;
                for (int i = 0; i < loop; i++)
                {
                    if (totalExperience - (10 * (Math.Pow(loop, 2)) + 500) > 0)
                    {
                        totalExperience = Convert.ToInt32(totalExperience - (10 * (Math.Pow(loop, 2)) + 500));
                        level++;
                    }
                    else if (totalExperience - (10 * (Math.Pow(loop, 2)) + 500) < 0)
                    {
                        currentExp = totalExperience;
                        levelExp = Convert.ToInt32((10 * (Math.Pow(loop, 2)) + 500));
                        return (level, currentExp, levelExp);
                    }
                    loop++;
                }
            }
            return (1, 0, 0);
        }

        public void addRole(IRole role, string serverid)
        {
            context.serverroles.Add(new serverroles()
            {
                serverid = serverid,
                roleid = role.Id.ToString(),
                rolename = role.Name
            });
            context.SaveChanges();
            return;
        }
        public void removeRole(IRole role, string serverid)
        {
            var res = context.serverroles.FirstOrDefault(x => x.serverid == serverid && x.rolename == role.Name);
            context.serverroles.Remove(res);
            context.SaveChanges();
            return;
        }
        public string getServerRole(string serverid)
        {
            List<string> roles = context.serverroles.Where(x => x.serverid == serverid).Select(x => x.rolename).ToList();
            var fullist = string.Join(", ", roles);
            return fullist;
        }

        public bool checkServerRole(string serverid, string roleid)
        {
            if (context.serverroles.Count(x => x.serverid == serverid && x.roleid == roleid) > 0)
            {
                return true;
            }
            return false;
        }
        public void addUser(SocketUser user, bool isBot = false)
        {
            if(user.IsBot != isBot)
            {
                return;
            }
            var res = context.users.FirstOrDefault(x => x.id == user.Id.ToString());
            if(res != null)
            {

            } else
            {
                context.users.Add(new users()
                {
                    id = user.Id.ToString(),
                    avatarID = user.AvatarId.ToString(),
                    avatarUrl = user.GetAvatarUrl(ImageFormat.Auto, 128),
                    discriminator = int.Parse(user.Discriminator),
                    email = null,
                    created_at = DateTime.Now,
                    expire_at = DateTime.Now,
                    updated_at = DateTime.Now,
                    token = null,
                    name = user.Username.ToString()

                });
            }
        }
    }
}