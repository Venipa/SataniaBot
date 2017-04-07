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

                var command = new MySqlCommand($"SELECT * FROM userstats where userid = @userid;", conn);
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


    }
}