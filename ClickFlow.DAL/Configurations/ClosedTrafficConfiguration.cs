using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Configurations
{
    public class ClosedTrafficConfiguration : IEntityTypeConfiguration<ClosedTraffic>
    {
        public void Configure(EntityTypeBuilder<ClosedTraffic> builder)
        {
            builder.ToTable("ClosedTraffics");
            builder.HasKey(ct => ct.Id);
            builder.Property(ct => ct.Id).UseIdentityColumn();

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

            builder.HasOne(ct => ct.CampaignParticipation)
                .WithMany(cp => cp.ClosedTraffics)
                .HasForeignKey(ct => ct.CampaignParticipationId)
                .IsRequired(false);
        }
    }
}
