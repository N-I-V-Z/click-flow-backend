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
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).UseIdentityColumn();

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(c => c.Description)
                   .IsRequired();
            builder.Property(c => c.OriginURL)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(c => c.Budget)
                   .IsRequired();
            builder.Property(c => c.StartDate)
                   .IsRequired();
            builder.Property(c => c.EndDate)
                   .IsRequired();
            builder.Property(c => c.TypePay)
                   .IsRequired();
            builder.Property(c => c.TypeCampaign)
                   .IsRequired();
            builder.Property(c => c.Status)
                   .IsRequired();
            builder.Property(c => c.Image)
                   .HasMaxLength(500);
            builder.Property(c => c.IsDeleted)
                   .IsRequired();

            // Quan hệ với Advertiser (nếu có)
            builder.HasOne(c => c.Advertiser)
                   .WithMany(a => a.Campaigns)
                   .HasForeignKey(c => c.AdvertiserId)
                   .IsRequired(false);

            // Quan hệ one-to-many với Traffic, ClosedTraffic, Feedback, Report
            builder.HasMany(c => c.Traffics)
                   .WithOne(t => t.Campaign)
                   .HasForeignKey(t => t.CampaignId)
                   .IsRequired(false);
            builder.HasMany(c => c.ClosedTraffics)
                   .WithOne(ct => ct.Campaign)
                   .HasForeignKey(ct => ct.CampaignId)
                   .IsRequired(false);
            builder.HasMany(c => c.Feedbacks)
                   .WithOne(f => f.Campaign)
                   .HasForeignKey(f => f.CampaignId)
                   .IsRequired(false);
            builder.HasMany(c => c.Reports)
                   .WithOne(r => r.Campaign)
                   .HasForeignKey(r => r.CampaignId)
                   .IsRequired(false);
        }
    }
}
