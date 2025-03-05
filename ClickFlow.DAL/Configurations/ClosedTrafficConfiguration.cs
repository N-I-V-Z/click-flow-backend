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
                   .HasMaxLength(50);
            builder.Property(ct => ct.OrderId)
                   .HasMaxLength(100);
            builder.Property(ct => ct.Browser)
                   .HasMaxLength(100);
            builder.Property(ct => ct.ReferrerURL)
                   .HasMaxLength(500);
            builder.Property(ct => ct.Timestamp)
                   .IsRequired();

            builder.HasOne(ct => ct.Campaign)
                   .WithMany(c => c.ClosedTraffics)
                   .HasForeignKey(ct => ct.CampaignId)
                   .IsRequired(false);
            builder.HasOne(ct => ct.Publisher)
                   .WithMany(p => p.ClosedTraffics)
                   .HasForeignKey(ct => ct.PublisherId)
                   .IsRequired(false);
        }
    }
}
