using System;
using MySql.Data.MySqlClient;
using Discord.WebSocket;
using System.Linq;

namespace SataniaBot.Services
{
    public class DatabaseHandler
    {
        static string host = Configuration.Load().DatabaseHost;
        static string username = Configuration.Load().DatabaseUsername;
        static string password = Configuration.Load().DatabasePassword;
        static string dbname = Configuration.Load().DatabaseName;

        string myConnectionString = $"server=" + host + "; uid=" + username + "; pwd=" + password + "; database=" + dbname + ";";



        public string getPrefix(string guildid)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection();

                conn.ConnectionString = myConnectionString;

                conn.Open();

                var command = new MySqlCommand($"SELECT * FROM serversettings where serverid = @guildid;", conn);
                command.Parameters.AddWithValue("@guildid", guildid);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string result = reader["commandprefix"].ToString();

                    conn.Close();

                    return result;
                }
                conn.Close();
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

        public void updatePrefix(SocketGuild s, string prefix)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"UPDATE `serversettings` SET `commandprefix`= @prefix WHERE `serverid`= @guildid;", conn);
            command.Parameters.AddWithValue("@guildid", s.Id);
            command.Parameters.AddWithValue("@prefix", prefix);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();
        }


        public void addServer(SocketGuild s)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"INSERT INTO `serversettings` (`serverid`, `commandprefix`) VALUES ({s.DefaultChannel.Id.ToString()}, 's?')", conn);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();

            updateWebStats(Satania._client.Guilds.Count, Satania._client.Guilds.SelectMany(x => x.Channels).Count(), Satania._client.Guilds.SelectMany(x => x.Users).Count());

            return;
        }

        public string getMarriage(string userid)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection();

                conn.ConnectionString = myConnectionString;

                conn.Open();

                var command = new MySqlCommand($"SELECT marriedid FROM usermarriages where userid = @userid;", conn);
                command.Parameters.AddWithValue("@userid", userid);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var result = reader["marriedid"].ToString();

                    conn.Close();

                    return result;
                }
                conn.Close();
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
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"INSERT INTO `usermarriages` (`userid`, `marriedid`) VALUES (@person1, @person2)", conn);
            command.Parameters.AddWithValue("@person1", person1);
            command.Parameters.AddWithValue("@person2", person2);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();

            conn.Open();

            var command2 = new MySqlCommand($"INSERT INTO `usermarriages` (`userid`, `marriedid`) VALUES (@person2, @person1)", conn);
            command2.Parameters.AddWithValue("@person2", person2);
            command2.Parameters.AddWithValue("@person1", person1);

            MySqlDataReader reader2 = command2.ExecuteReader();

            conn.Close();
        }

        public void removeMarriage(string person1, string person2)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"DELETE FROM `usermarriages` WHERE `userid`=@person1", conn);
            command.Parameters.AddWithValue("@person1", person1);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();

            conn.Open();

            var command2 = new MySqlCommand($"DELETE FROM `usermarriages` WHERE `userid`=@person2", conn);
            command2.Parameters.AddWithValue("@person2", person2);

            MySqlDataReader reader2 = command2.ExecuteReader();

            conn.Close();
        }

        public void updateWebStats(int servernum, int channelnum, int usernum)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"UPDATE `usagestats` SET `servercount`=@server, `channelcount`=@channels, `usercount`=@user;", conn);
            command.Parameters.AddWithValue("@server", servernum);
            command.Parameters.AddWithValue("@channels", channelnum);
            command.Parameters.AddWithValue("@user", usernum);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();
        }

        public void incrementCommands()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"UPDATE `usagestats` SET `commandusage`= `commandusage` + 1", conn);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();
        }

        public void addNsfwChannel(string channelid)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            var command2 = new MySqlCommand($"INSERT INTO `nsfwchannels` (`channelid`) VALUES ({channelid})", conn);

            MySqlDataReader reader2 = command2.ExecuteReader();

            conn.Close();
        }

        public void removeNsfwChannel(string channelid)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

            var command2 = new MySqlCommand($"DELETE FROM `nsfwchannels` WHERE `channelid` = {channelid}", conn);

            MySqlDataReader reader2 = command2.ExecuteReader();

            conn.Close();
        }

        public bool checkNsfw(string channelid)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection();

                conn.ConnectionString = myConnectionString;

                conn.Open();

                var command = new MySqlCommand($"SELECT channelid FROM nsfwchannels where channelid = {channelid};", conn);

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    conn.Close();

                    return true;
                }
                conn.Close();
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

            MySqlConnection conn = new MySqlConnection();

            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"SELECT lastmessage FROM experiencetimers where userid = @userid;", conn);
            command.Parameters.AddWithValue("@userid", userid);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var result = reader["lastmessage"];

                conn.Close();

                return result as DateTime?;
            }
            conn.Close();
            return null;
        }

        public void updateTimer(string userid)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            DateTime Now = DateTime.Now;
            if (getTimer(userid) == null)
            {
                conn.Open();
                var command = new MySqlCommand($"INSERT INTO `experiencetimers` (`userid`, `lastmessage`) VALUES (@person, @time)", conn);
                command.Parameters.AddWithValue("@person", userid);
                command.Parameters.AddWithValue("@time", Now);
                MySqlDataReader reader = command.ExecuteReader();
                conn.Close();
            }
            else
            {
                conn.Open();
                var command = new MySqlCommand($"UPDATE `experiencetimers` SET `lastmessage`=@time WHERE `userid`=@user;", conn);
                command.Parameters.AddWithValue("@time", Now);
                command.Parameters.AddWithValue("@user", userid);
                MySqlDataReader reader = command.ExecuteReader();
                conn.Close();
            }
        }



        public void incrementExperience(string userid, int experiencegain)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"SELECT * FROM userexperience where userid = {userid};", conn);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                conn.Close();
                conn.Open();

                var command3 = new MySqlCommand($"UPDATE `userexperience` SET `experience`= `experience` + {experiencegain} WHERE `userid`={userid}", conn);

                MySqlDataReader reader3 = command3.ExecuteReader();

                conn.Close();
                return;
            }
            conn.Close();

            conn.Open();
            var command2 = new MySqlCommand($"INSERT INTO `userexperience` (`userid`, `experience`) VALUES ({userid}, {experiencegain})", conn);

            MySqlDataReader reader2 = command2.ExecuteReader();

            conn.Close();
            return;
        }

        public int? getExperience(string userid)
        {

            MySqlConnection conn = new MySqlConnection();

            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"SELECT experience FROM userexperience where userid = @userid;", conn);
            command.Parameters.AddWithValue("@userid", userid);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var result = reader["experience"];

                conn.Close();

                return result as int?;
            }
            conn.Close();
            return 0;
        }
    }
}