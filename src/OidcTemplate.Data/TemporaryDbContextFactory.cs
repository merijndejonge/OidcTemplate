using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenSoftware.OidcTemplate.Data
{
    public class TemporaryDbContextFactory: IDesignTimeDbContextFactory<IdentityContext>
    {
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Auth;Trusted_Connection=True;MultipleActiveResultSets=true";

        public IdentityContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<IdentityContext>();
            builder.UseSqlServer(_connectionString,
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(DataModule).GetTypeInfo().Assembly.GetName().Name));
            return new IdentityContext(builder.Options);
        }
    }
    public class TemporaryDbContextFactoryScopes : IDesignTimeDbContextFactory<PersistedGrantDbContext>
    {
        private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Auth;Trusted_Connection=True;MultipleActiveResultSets=true";

        public PersistedGrantDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            builder.UseSqlServer(_connectionString,
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(DataModule).GetTypeInfo().Assembly.GetName().Name));
            return new PersistedGrantDbContext(builder.Options, new OperationalStoreOptions());
        }
    }
}