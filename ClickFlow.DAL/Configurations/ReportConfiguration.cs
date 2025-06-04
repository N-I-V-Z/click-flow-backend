using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class ReportConfiguration : IEntityTypeConfiguration<Report>
	{
		public void Configure(EntityTypeBuilder<Report> builder)
		{
			builder.ToTable("Reports");
			builder.HasKey(r => r.Id);
			builder.Property(r => r.Id).UseIdentityColumn();

			builder.Property(r => r.Reason)
				   .IsRequired()
				   .HasMaxLength(500);
			builder.Property(r => r.Status)
				   .IsRequired();
			builder.Property(r => r.CreateAt)
				   .IsRequired();
			builder.Property(r => r.Response)
				   .IsRequired(false)
				   .HasMaxLength(1000);
			builder.Property(r => r.EvidenceURL)
				   .HasMaxLength(500);

			builder.HasOne(r => r.Publisher)
				   .WithMany(p => p.Reports)
				   .HasForeignKey(r => r.PublisherId)
				   .IsRequired(false);
			builder.HasOne(r => r.Advertiser)
				   .WithMany(a => a.Reports)
				   .HasForeignKey(r => r.AdvertiserId)
				   .IsRequired(false);
			builder.HasOne(r => r.Campaign)
				   .WithMany(c => c.Reports)
				   .HasForeignKey(r => r.CampaignId)
				   .IsRequired(false);
		}
	}

}
