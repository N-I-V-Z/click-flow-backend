using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClickFlow.DAL.EF
{
    internal class ClickFlowContextFactory : IDesignTimeDbContextFactory<ClickFlowContext>
    {
        public ClickFlowContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ClickFlow.API");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ClickFlowContext>();
            var connectionString = configuration.GetConnectionString("ClickFlowDB");

            builder.UseSqlServer(connectionString);

            return new ClickFlowContext(builder.Options);
        }
    }


}
