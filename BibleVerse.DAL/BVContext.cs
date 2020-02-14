using Microsoft.EntityFrameworkCore;
using BibleVerse.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.DAL
{
    public class BVContext : DbContext
    {
        public BVContext(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=MAINPC;Database=BibleVerseOPP;Trusted_Connection=True;");
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

        public DbSet<Users> Users { get; set; }
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Organization> Organization { get; set; }
        public DbSet<OrgSettings> OrgSettings { get; set; }
        public DbSet<Photos> Photos { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<UserAssignments> UserAssignments { get; set; }
        public DbSet<UserCourses> UserCourses { get; set; }
        public DbSet<Videos> Videos { get; set; }
    }
}
