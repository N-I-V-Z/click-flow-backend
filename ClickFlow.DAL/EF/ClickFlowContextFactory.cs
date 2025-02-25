using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClickFlow.DAL.EF
{
    internal class ClickFlowContextFactory : IDesignTimeDbContextFactory<ClickFlowContext>
    {
        public ClickFlowContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ClickFlowContext>();
            var connectionString = configuration.GetConnectionString("ClickFlowDB");

            builder.UseSqlServer(connectionString);

            return new ClickFlowContext(builder.Options);
        }
    }


}
