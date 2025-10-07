using Microsoft.EntityFrameworkCore;
using RefugeeSkillsPlatform.Core.DTOs;
using RefugeeSkillsPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefugeeSkillsPlatform.Infrastructure.Persistence
{
    public class RefugeeSkillsDbContext : DbContext
    {
        public RefugeeSkillsDbContext(DbContextOptions<RefugeeSkillsDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Category { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<DeliveryMethods> DeliveryMethods { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<AvailabilitySlots> AvailabilitySlots { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<Payments> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfileResponse>().HasNoKey();  // 👈 keyless entity
            modelBuilder.Entity<ServiceResponse>().HasNoKey();
            modelBuilder.Entity<ServiceSlotResponse>().HasNoKey();
        }
    }
}
