using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace BibleVerse.DTO
{
    public class BVIdentityContext : IdentityDbContext<Users>
    {
        public BVIdentityContext(DbContextOptions<BVIdentityContext> options) : base(options) { }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BVIdentityContext>
        {
            public BVIdentityContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../BibleVerseAPI/appsettings.json").Build();
                var builder = new DbContextOptionsBuilder<BVIdentityContext>();
                var connectionString = configuration.GetConnectionString("DatabaseConnection");
                builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("BibleVerseAPI"));
                return new BVIdentityContext(builder.Options);
            }

        }


        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<Users>();
            mb.Entity<Assignments>();
            mb.Entity<Courses>();
            mb.Entity<Messages>();
            mb.Entity<Organization>();
            mb.Entity<OrgSettings>();
            mb.Entity<Photos>();
            mb.Entity<Posts>();
            mb.Entity<Profiles>();
            mb.Entity<UserAssignments>();
            mb.Entity<UserCourses>();
            mb.Entity<Videos>();
            mb.Entity<ELog>();
            mb.Entity<UserAWS>();
            mb.Entity<UserRelationships>();
            mb.Entity<UserHistory>();
            mb.Entity<Notifications>();
            mb.Entity<Subscriptions>();
            mb.Entity<SubscriptionsHistory>();
            mb.Entity<RefCodeLogs>();
            mb.Entity<SiteConfigs>();
            mb.Entity<RefreshToken>();
            mb.Entity<Likes>();
            mb.Entity<Comments>();
        }

        public virtual DbSet<Users> BVUsers { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<OrgSettings> OrgSettings { get; set; }
        public virtual DbSet<Photos> Photos { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<Profiles> Profiles { get; set; }
        public virtual DbSet<UserAssignments> UserAssignments { get; set; }
        public virtual DbSet<UserCourses> UserCourses { get; set; }
        public virtual DbSet<Videos> Videos { get; set; }
        public virtual DbSet<ELog> ELogs { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<UserHistory> UserHistory { get; set; }
        public virtual DbSet<UserAWS> UserAWS { get; set; }
        public virtual DbSet<UserRelationships> UserRelationships { get; set; }
        public virtual DbSet<Subscriptions> Subscriptions { get; set; }
        public virtual DbSet<SubscriptionsHistory> SubscriptionsHistory { get; set; }
        public virtual DbSet<RefCodeLogs> RefCodeLogs { get; set; }
        public virtual DbSet<SiteConfigs> SiteConfigs { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Likes> Likes { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
    }
}
