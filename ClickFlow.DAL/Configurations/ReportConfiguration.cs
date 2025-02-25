using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class ReportConfiguration : IEntityTypeConfiguration<Reports>
	{
		public void Configure(EntityTypeBuilder<Reports> builder)
		{
			builder.ToTable("Reports");
			builder.HasKey(r => r.Id);

			builder.Property(r => r.Id).UseIdentityColumn();
			builder.Property(r => r.Reason).IsRequired().HasMaxLength(255).IsUnicode();
			builder.Property(r => r.Status).IsRequired();
			builder.Property(r => r.CreateAt).IsRequired();
			builder.Property(r => r.EvidenceURL).IsRequired().HasMaxLength(255);

			builder.HasOne(c => c.Campaign).WithMany(r => r.Reports).HasForeignKey(r => r.CampaignId);
			builder.HasOne(u => u.Publisher).WithMany(r => r.Reports).HasForeignKey(r => r.PublisherId);
			builder.HasOne(u => u.Advertiser).WithMany(r => r.Reports).HasForeignKey(r => r.AdvertiserId);
		}
	}
}
