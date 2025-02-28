using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).UseIdentityColumn();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100).IsUnicode();
            builder.Property(c => c.Description).IsRequired().HasMaxLength(255).IsUnicode();
            builder.Property(c => c.OriginURL).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Budget).IsRequired();
            builder.Property(c => c.StartDate).IsRequired();
            builder.Property(c => c.EndDate).IsRequired();
            builder.Property(c => c.TypePay).IsRequired();
            builder.Property(c => c.TypeCampaign).IsRequired();
            builder.Property(c => c.Commission).IsRequired(false);
            builder.Property(c => c.Percents).IsRequired(false);
            builder.Property(c => c.Image).IsRequired().HasMaxLength(255);

            builder.HasOne(a => a.Advertiser).WithMany(c => c.Campaigns).HasForeignKey(c => c.AdvertiserId);
        }
    }
}
