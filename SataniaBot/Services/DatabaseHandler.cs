using System;
using MySql.Data.MySqlClient;
using Discord.WebSocket;

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
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
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

            var command = new MySqlCommand($"INSERT INTO `serversettings` (`serverid`, `commandprefix`) VALUES (@guildid', '!')", conn);
            command.Parameters.AddWithValue("@guildid", s.Id);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();
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


    }
}