using System;
using System.Collections.Generic;
using System.Text;
using MySql;
using MySql.Data.MySqlClient;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TromoBot.Services
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

                var command = new MySqlCommand($"SELECT * FROM serversettings where serverid = '{guildid}';", conn);

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

        public void addServer(SocketGuild s)
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;

            conn.Open();

            var command = new MySqlCommand($"INSERT INTO `serversettings` (`serverid`, `commandprefix`) VALUES ('{s.Id}', '!')", conn);

            MySqlDataReader reader = command.ExecuteReader();

            conn.Close();
        }

    }
}