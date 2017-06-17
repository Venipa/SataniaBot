using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SataniaBot.Services;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SataniaBot.Database.Tables
{

    public class MySqlContext : DbContext
    {
        static string host = Configuration.Load().DatabaseHost;
        static string username = Configuration.Load().DatabaseUsername;
        static string password = Configuration.Load().DatabasePassword;
        static string dbname = Configuration.Load().DatabaseName;
        public DbSet<users> users { get; set; }
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
        public DbSet<usermoney> usermoney { get; set; }
        public DbSet<userdailytimer> userdaily { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySql($@"Server={host};Port=3306;Database={dbname};Uid={username};Pwd={password};charset=utf8;");
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            foreach (var relationship in modelbuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            base.OnModelCreating(modelbuilder);
        }
    }

    public class users
    {
        [Key, MaxLength(30)]
        public string id { get; set; }
        [Required, MaxLength(250)]
        public string name { get; set; }
        public string email { get; set; }
        [Required]
        public int discriminator { get; set; }
        public string token { get; set; }
        public string avatarUrl { get; set; }
        public string avatarID { get; set; }
        public string remember_token { get; set; }

        public DateTime expire_at { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

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
        [MaxLength(10), Required]
        public string currencyname { get; set; }
    }
    public class usagestats
    {
        [MaxLength(11), Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [MaxLength(50), Key, Required]
        public string userid { get; set; }

        [MaxLength(11), Required]
        public int reps { get; set; }
    }
    public class userreptimers
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }

        public DateTime lastrep { get; set; }
    }
    public class userdailytimer
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }

        public DateTime lastdaily { get; set; }
    }
    public class usermoney
    {
        [MaxLength(50), Key, Required]
        public string userid { get; set; }
        [Required]
        public uint money { get; set; }
    }
}
