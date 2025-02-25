using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class TrafficConfiguration : IEntityTypeConfiguration<Traffic>
    {
        public void Configure(EntityTypeBuilder<Traffic> builder)
        {
            builder.ToTable("Traffics").HasIndex(t => t.IpAddress);
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).UseIdentityColumn();
            builder.Property(t => t.IpAddress).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Timestamp).IsRequired();
            builder.Property(t => t.IsValid).IsRequired(false);
            builder.Property(t => t.Revenue).IsRequired(false);
            builder.Property(t => t.DeviceType).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Browser).IsRequired().HasMaxLength(100);
            builder.Property(t => t.ReferrerURL).IsRequired().HasMaxLength(255);
            builder.Property(t => t.OrderId).IsRequired(false);

            builder.HasOne(u => u.Publisher).WithMany(t => t.Traffics).HasForeignKey(t => t.PublisherId);
            builder.HasOne(u => u.Campaign).WithMany(t => t.Traffics).HasForeignKey(t => t.CampaignId);
        }
    }
}
