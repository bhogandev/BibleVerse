using Microsoft.EntityFrameworkCore;
using BibleVerse.DTO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using BibleVerseAPI.Models;

namespace BibleVerse.DAL
{
    public class BVContext : DbContext
    {
        
        public BVContext(DbContextOptions<BVContext> options) : base(options) { }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BVContext>
        {
            public BVContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../BibleVerseAPI/appsettings.json").Build();
                var builder = new DbContextOptionsBuilder<BVContext>();
                var connectionString = configuration.GetConnectionString("DatabaseConnection");
                builder.UseSqlServer(connectionString);
                return new BVContext(builder.Options);
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
        }
        
        public virtual DbSet<Users> Users { get; set; }
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
    }
}
