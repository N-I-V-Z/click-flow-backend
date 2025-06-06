using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class CampaignParticipationConfiguration : IEntityTypeConfiguration<CampaignParticipation>
	{
		public void Configure(EntityTypeBuilder<CampaignParticipation> builder)
		{
			builder.ToTable("CampaignParticipations");
			builder.HasKey(cp => cp.Id);
			builder.Property(cp => cp.Id).UseIdentityColumn();

			builder.Property(cp => cp.Status).IsRequired();
			builder.Property(cp => cp.CreateAt).IsRequired();
			builder.Property(cp => cp.ShortLink).IsRequired(false).HasMaxLength(100);

			builder.HasOne(cp => cp.Publisher)
				.WithMany(p => p.CampaignParticipations)
				.HasForeignKey(cp => cp.PublisherId);
			builder.HasOne(cp => cp.Campaign)
				.WithMany(c => c.CampaignParticipations)
				.HasForeignKey(cp => cp.CampaignId);
		}
	}
}
