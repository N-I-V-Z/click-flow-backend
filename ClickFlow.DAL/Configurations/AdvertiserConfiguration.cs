using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class AdvertiserConfiguration : IEntityTypeConfiguration<Advertiser>
    {
        public void Configure(EntityTypeBuilder<Advertiser> builder)
        {
            builder.ToTable("Advertisers");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).UseIdentityColumn();
            builder.Property(a => a.Industry).IsRequired();
            builder.Property(a => a.CompanyName).IsRequired().HasMaxLength(100).IsUnicode();
        }
    }
}
