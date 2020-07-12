using Coiner.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coiner.Business.Context
{

    public class CoinerContext : DbContext
    {
        IConfigurationSection connectionString;
        public CoinerContext()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json", true, true)
                                   .Build();

            connectionString = config.GetSection("ConnectionStrings");
        }

        public CoinerContext(DbContextOptions<CoinerContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Coin> Coins { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<ProjectImage> ProjectImages { get; set; }

        public DbSet<UserImage> UserImage { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<ProjectUpdate> ProjectUpdate { get; set; }

        public DbSet<Discussion> Discussions { get; set; }

        public DbSet<UserWallet> UserWallet { get; set; }

        public DbSet<NotificationConfiguration> NotificationConfiguration { get; set; }

        public DbSet<NotificationProduced> NotificationProduced { get; set; }

        public DbSet<NotificationSent> NotificationSent { get; set; }

        public DbSet<NotificationTemplate> NotificationTemplate { get; set; }

        public DbSet<Bill> Bills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql(connectionString["DefaultConnection"]);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(connectionString["DefaultConnection"]);
            }
        }
    }
}
