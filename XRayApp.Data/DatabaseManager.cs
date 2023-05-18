using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using XRayApp.Data.Interfaces;
using XRayApp.Data.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using XRayApp.Data.Utility;

namespace XRayApp.Data
{
    public class DatabaseManager: DbContext
    {
        //public DatabaseManager(DbContextOptions<DatabaseManager> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<Image> Images { get; set; }

        public IPatientRepository PatientsRepository { get; }
        public IStudyRepository StudiesRepository { get; }
        public IImageRepository ImagesRepository { get; }

        private readonly IConfiguration configuration;

        public DatabaseManager()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            PatientsRepository = new PatientRepository(this);
            StudiesRepository = new StudyRepository(this);
            ImagesRepository = new ImageRepository(this);
        }
        ~DatabaseManager()
        {
            // Создание дампа базы данных
            //var utility = new DatabaseUtility();
            //utility.CreateDatabaseDump("database_dump.sql");
        }

        public void InitializeDatabase()
        {
            Database.EnsureCreated();

            if (!Patients.Any() && !Studies.Any() && !Images.Any())
            {
                var utility = new DatabaseUtility();
                utility.RestoreDatabaseFromDump("database_dump.sql");
                SaveChanges();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(5, 5, 26)));         
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.Id);            

            modelBuilder.Entity<Study>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Image>()
                .HasKey(i => i.Id);

            // Определение связей между моделями данных
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Studies)
                .WithOne(s => s.Patient)
                .HasForeignKey(s => s.PatientId);

            modelBuilder.Entity<Study>()
                .HasMany(s => s.Images)
                .WithOne(i => i.Study)
                .HasForeignKey(i => i.StudyId);

            //Seed(modelBuilder);
        }

        private void Seed()
        {
            for (int i = 1; i <= 50; i++)
            {
                var patient = new Patient
                {
                    Id = i,
                    PatientId = $"Patient-{i}",
                    FirstName = $"First Name-{i}",
                    LastName = $"Last Name-{i}",
                    MiddleName = $"Middle Name-{i}",
                    BirthDate = DateTime.Now.AddYears(-30).AddDays(i), // Just for variety
                    Gender = i % 2 == 0 ? "Male" : "Female",
                    Address = $"Address-{i}",
                    Comment = $"Comment-{i}",
                    Studies = new List<Study>()
                };

                Patients.Add(patient);

                var study = new Study
                {
                    Id = i,
                    StudyId = $"Study-{i}",
                    StudyDate = DateTime.Now.AddDays(-i), // Just for variety
                    Description = $"Description-{i}",
                    PatientId = i,
                    Patient = patient,
                    Images = new List<Image>()
                };

                patient.Studies.Add(study);
                Studies.Add(study);

                var image = new Image
                {
                    Id = i,
                    ImageId = $"Image-{i}",
                    ImageDate = DateTime.Now.AddHours(-i), // Just for variety
                    ImagePath = $"Path-{i}",
                    SeriesUID = $"SeriesUID-{i}",
                    StudyUID = $"StudyUID-{i}",
                    ExposureParameters = $"ExposureParameters-{i}",
                    StudyId = i,
                    Study = study
                };

                study.Images.Add(image);
                Images.Add(image);
            }
        }


    }
}
