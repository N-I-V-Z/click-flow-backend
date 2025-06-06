using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class TrafficConfiguration : IEntityTypeConfiguration<Traffic>
    {
        public void Configure(EntityTypeBuilder<Traffic> builder)
        {
            builder.ToTable("Traffics");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseIdentityColumn();

            builder.Property(ct => ct.IpAddress)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(ct => ct.DeviceType)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(ct => ct.OrderId)
                    .IsRequired(false)
                   .HasMaxLength(100);
            builder.Property(ct => ct.Browser)
                   .IsRequired(false)
                   .HasMaxLength(100);
            builder.Property(ct => ct.ReferrerURL)
                   .IsRequired(false)
                   .HasMaxLength(500);
            builder.Property(ct => ct.Timestamp)
                   .IsRequired();

            builder.HasOne(t => t.CampaignParticipation)
                   .WithMany(cp => cp.Traffics)
                   .HasForeignKey(t => t.CampaignParticipationId)
                   .IsRequired(false);
        }
    }

}
