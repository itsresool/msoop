using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Msoop.Core.Models;
using Msoop.Infrastructure.Data;
using Npgsql;
using Xunit;

namespace IntegrationTests
{
    [CollectionDefinition("MsoopTests")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }

    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            var config = GetConfiguration();
            Connection = new NpgsqlConnection(config.GetConnectionString("MsoopConnection"));

            Seed();
        }

        public DbConnection Connection { get; }

        public void Dispose()
        {
            Connection?.Dispose();
        }

        public MsoopContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MsoopContext>().UseNpgsql(Connection);
            return new MsoopContext(optionsBuilder.Options);
        }

        public void Seed()
        {
            using var context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var sheetOne = new Sheet
            {
                Id = new Guid(),
                PostAgeLimitInDays = 7,
                Subreddits = new List<Subreddit>(new Subreddit[]
                {
                    new() { Name = "soccer", MaxPostCount = 20, PostOrdering = PostOrdering.Newest },
                    new() { Name = "dotnet", MaxPostCount = 10, PostOrdering = PostOrdering.Newest },
                }),
            };

            var sheetTwo = new Sheet
            {
                Id = new Guid(),
                PostAgeLimitInDays = 14,
            };

            var sheetThree = new Sheet
            {
                Id = new Guid(),
                PostAgeLimitInDays = 7,
                Subreddits = new List<Subreddit>(new Subreddit[]
                {
                    new() { Name = "csharp", MaxPostCount = 20, PostOrdering = PostOrdering.Oldest },
                    new() { Name = "webdev", MaxPostCount = 10, PostOrdering = PostOrdering.ScoreDesc },
                    new() { Name = "programming", MaxPostCount = 10, PostOrdering = PostOrdering.CommentsDesc },
                }),
            };

            context.AddRange(sheetOne, sheetTwo, sheetThree);
            context.SaveChanges();
        }

        private IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@"appsettings.Testing.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
