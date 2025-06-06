using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class PlanConfiguration : IEntityTypeConfiguration<Plan>
	{
		public void Configure(EntityTypeBuilder<Plan> builder)
		{
			builder.ToTable("Plans");

			builder.HasKey(p => p.Id);

			builder.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(p => p.MaxCampaigns)
				.IsRequired();

			builder.Property(p => p.MaxClicksPerMonth)
				.IsRequired();

			builder.Property(p => p.MaxConversionsPerMonth)
				.IsRequired();

			builder.Property(p => p.IsActive)
				.HasDefaultValue(true);

			builder.Property(p => p.Description)
				.HasMaxLength(500);

			builder.HasMany(p => p.UserPlans)
				.WithOne(up => up.Plan)
				.HasForeignKey(up => up.PlanId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}