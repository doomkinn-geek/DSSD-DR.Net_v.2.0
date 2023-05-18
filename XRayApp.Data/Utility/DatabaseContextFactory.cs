using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace XRayApp.Data.Utility
{
    // Фабрика контекста базы данных, необходима для создания миграций
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseManager>
    {
        private readonly IConfiguration configuration;

        public DatabaseContextFactory()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public DatabaseManager CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseManager>();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(5, 5, 26)));

            return new DatabaseManager();
        }
    }
}
