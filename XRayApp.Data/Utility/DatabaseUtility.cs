using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRayApp.Data.Models;

namespace XRayApp.Data.Utility
{
    public class DatabaseUtility
    {
        private readonly IConfiguration configuration;

        public DatabaseUtility()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public void CreateDatabaseDumpWithMysqlDump(string dumpFilePath)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var connectionBuilder = new MySqlConnectionStringBuilder(connectionString);

            var startInfo = new ProcessStartInfo
            {
                FileName = "mysqldump",
                Arguments = $"--user={connectionBuilder.UserID} --password={connectionBuilder.Password} --host={connectionBuilder.Server} {connectionBuilder.Database}",
                RedirectStandardOutput = true,
            };
            using var process = Process.Start(startInfo);
            using var reader = process.StandardOutput;
            var result = reader.ReadToEnd();

            File.WriteAllText(dumpFilePath, result);
        }

        public void CreateDatabaseDump(string dumpFilePath)
        {
            var sb = new StringBuilder();

            using (var dbContext = CreateDbContext())
            {
                // Сначала создаем записи в таблице patients
                var patients = dbContext.Patients.ToList();
                foreach (var patient in patients)
                {
                    sb.AppendLine(CreateInsertSql(patient, "patients"));
                }

                // Затем в таблице studies
                var studies = dbContext.Studies.ToList();
                foreach (var study in studies)
                {
                    sb.AppendLine(CreateInsertSql(study, "studies"));
                }

                // И наконец, в таблице images
                var images = dbContext.Images.ToList();
                foreach (var image in images)
                {
                    sb.AppendLine(CreateInsertSql(image, "images"));
                }
            }

            // Записываем все SQL инструкции в файл
            File.WriteAllText(dumpFilePath, sb.ToString());
        }

        private string CreateInsertSql(object entity, string tableName)
        {
            var entityType = entity.GetType();

            // Получаем только те свойства, которые соответствуют столбцам базы данных
            var properties = entityType.GetProperties()
                .Where(p => p.CanRead
                            && p.CanWrite
                            && p.PropertyType.IsValueType
                            || p.PropertyType == typeof(string));

            var columnNames = string.Join(", ", properties.Select(p => $"`{p.Name}`"));

            var values = string.Join(", ", properties.Select(p =>
            {
                var value = p.GetValue(entity);

                // Если это DateTime, форматируем значение
                if (p.PropertyType == typeof(DateTime))
                {
                    DateTime dt = (DateTime)value;
                    return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
                }

                // Если это DateTime?, форматируем значение, если оно не null
                if (p.PropertyType == typeof(DateTime?))
                {
                    DateTime? dt = (DateTime?)value;
                    return dt.HasValue ? $"'{dt.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL";
                }

                // Для всех других типов просто возвращаем значение
                return $"'{value}'";
            }));

            return $"INSERT INTO `{tableName}` ({columnNames}) VALUES ({values});";
        }






        public void RestoreDatabaseFromDump(string dumpFilePath)
        {
            using (var dbContext = CreateDbContext())
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                var script = File.ReadAllText(dumpFilePath);

                dbContext.Database.ExecuteSqlRaw(script);
            }
        }

        private DatabaseManager CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseManager>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(5, 5, 26)));

            return new DatabaseManager();
        }
    }
}
