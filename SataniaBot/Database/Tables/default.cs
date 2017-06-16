using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SataniaBot.Services;

namespace SataniaBot.Database.Tables
{

    public class MySqlContext : DbContext
    {
        static string host = Configuration.Load().DatabaseHost;
        static string username = Configuration.Load().DatabaseUsername;
        static string password = Configuration.Load().DatabasePassword;
        static string dbname = Configuration.Load().DatabaseName;
        public DbSet<commandlist> commandlist { get; set; }
        public DbSet<experiencetimers> experiencetimers { get; set; }
        public DbSet<logchannels> logchannels { get; set; }
        public DbSet<nsfwchannels> nsfwchannels { get; set; }
        public DbSet<serverroles> serverroles { get; set; }
        public DbSet<serversettings> serversettings { get; set; }
        public DbSet<usagestats> usagestats { get; set; }
        public DbSet<userexperience> userexperience { get; set; }
        public DbSet<usermarriages> usermarriages { get; set; }
        public DbSet<userrep> userrep { get; set; }
        public DbSet<userreptimers> userreptimers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseMySql($@"Server={host};Port=3306;Database={dbname};Uid={username};Pwd={password};charset=utf8;");
    }
    public class commandlist
    {
        [MaxLength(150)]
        public string modulename { get; set; }
        [MaxLength(150), Key, Required]
        public string commandname { get; set; }
        [MaxLength(150)]
        public string commanddescription { get; set; }
        [MaxLength(150)]
        public string commandexample { get; set; }
    }
    public class experiencetimers
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }
        
        public DateTime lastmessage { get; set; }
    }
    public class logchannels
    {
        [MaxLength(50), Key, Required]
        public string serverid { get; set; }
        [MaxLength(50), Required]
        public string channelid { get; set; }
    }
    public class nsfwchannels
    {
        [MaxLength(50), Key, Required]
        public string channelid { get; set; }
    }
    public class serverroles
    {
        [MaxLength(45), Required]
        public string serverid { get; set; }
        [MaxLength(45), Key, Required]
        public string roleid { get; set; }
        [MaxLength(45), Required]
        public string rolename { get; set; }
    }
    public class serversettings
    {
        [MaxLength(30), Key, Required]
        public string serverid { get; set; }
        [MaxLength(10), Required]
        public string commandprefix { get; set; }
    }
    public class usagestats
    {
        [MaxLength(11), Required, Key]
        public int key { get; set; }
        [MaxLength(11), Required]
        public int servercount { get; set; }
        [MaxLength(11), Required]
        public int channelcount { get; set; }
        [MaxLength(11), Required]
        public int usercount { get; set; }
        [MaxLength(11), Required]
        public int commandusage { get; set; }
    }
    public class userexperience
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }
        [MaxLength(11)]
        public int experience { get; set; }
    }
    public class usermarriages
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }
        [MaxLength(50)]
        public string marriedid { get; set; }
    }
    public class userrep
    {
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [MaxLength(50), Required]
        public string userid { get; set; }

        [MaxLength(50), Required]
        public string repid { get; set; }
    }
    public class userreptimers
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }

        public DateTime lastrep { get; set; }
    }
}
