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
    public class TrafficConfiguration : IEntityTypeConfiguration<Traffic>
    {
        public void Configure(EntityTypeBuilder<Traffic> builder)
        {
            builder.ToTable("Traffics");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseIdentityColumn();

            builder.Property(t => t.IpAddress)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(t => t.DeviceType)
                   .HasMaxLength(50);
            builder.Property(t => t.OrderId)
                   .HasMaxLength(100);
            builder.Property(t => t.Browser)
                   .HasMaxLength(100);
            builder.Property(t => t.ReferrerURL)
                   .HasMaxLength(500);
            builder.Property(t => t.Timestamp)
                   .IsRequired();

            builder.HasOne(t => t.Campaign)
                   .WithMany(c => c.Traffics)
                   .HasForeignKey(t => t.CampaignId)
                   .IsRequired(false);
            builder.HasOne(t => t.Publisher)
                   .WithMany(p => p.Traffics)
                   .HasForeignKey(t => t.PublisherId)
                   .IsRequired(false);
        }
    }

}
