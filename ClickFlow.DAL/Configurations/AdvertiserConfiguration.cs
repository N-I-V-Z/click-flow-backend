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
    public class AdvertiserConfiguration : IEntityTypeConfiguration<Advertiser>
    {
        public void Configure(EntityTypeBuilder<Advertiser> builder)
        {
            builder.ToTable("Advertisers");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).UseIdentityColumn();

            builder.Property(a => a.CompanyName)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(a => a.IntroductionWebsite)
                   .HasMaxLength(500);
            builder.Property(a => a.StaffSize)
                   .IsRequired();
            builder.Property(a => a.Industry)
                   .IsRequired();

            builder.HasMany(a => a.Campaigns)
                   .WithOne(c => c.Advertiser)
                   .HasForeignKey(c => c.AdvertiserId)
                   .IsRequired(false);

            builder.HasMany(a => a.Reports)
                   .WithOne(r => r.Advertiser)
                   .HasForeignKey(r => r.AdvertiserId)
                   .IsRequired(false);
        }
    }
}
