using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RentgenArmParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentgenArmParser
{
    public class SecondDbContext : DbContext
    {
        public DbSet<ARMPatient> Patients { get; set; }
        public DbSet<ARMStudy> Studies { get; set; }
        public DbSet<ARMPicture> Pictures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            string connectionString = "Server=localhost;Port=3306;Database=rentgen_arm_mark_test;uid=root;Pwd=123456";
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 5, 26)));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ARMPatient>().HasKey(p => p.id_patient);
            modelBuilder.Entity<ARMStudy>().HasKey(s => s.id_study);
            modelBuilder.Entity<ARMPicture>().HasKey(i => i.id_picture);

            modelBuilder.Entity<ARMPicture>()
                .Property(p => p.kv)
                .HasConversion<uint>();

            modelBuilder.Entity<ARMPicture>()
                .Property(p => p.SID)
                .HasConversion<uint>();

            modelBuilder.Entity<ARMPicture>()
                .Property(p => p.ma)
                .HasConversion<float>();

            modelBuilder.Entity<ARMPicture>()
                .Property(p => p.ms)
                .HasConversion<float>();
        }       

    }   
}
