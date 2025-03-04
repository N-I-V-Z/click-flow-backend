using ClickFlow.DAL.Configurations;
using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClickFlow.DAL.EF
{
    public partial class ClickFlowContext : DbContext
    {

        public ClickFlowContext(DbContextOptions<ClickFlowContext> options)
        : base(options)
        {
        }

        #region DbSet

        #endregion

        public DbSet<Advertiser> Advertisers { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Traffic> Traffics { get; set; }
        public DbSet<ClosedTraffic> ClosedTraffics { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdvertiserConfiguration());
            modelBuilder.ApplyConfiguration(new CampaignConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new PublisherConfiguration());
            modelBuilder.ApplyConfiguration(new TrafficConfiguration());
            modelBuilder.ApplyConfiguration(new ClosedTrafficConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new WalletConfiguration());
            modelBuilder.ApplyConfiguration(new ReportConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodConfiguration());
        }
    }

}
