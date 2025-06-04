using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class UserPlanConfiguration : IEntityTypeConfiguration<UserPlan>
	{
		public void Configure(EntityTypeBuilder<UserPlan> builder)
		{
			builder.ToTable("UserPlans");

			builder.HasKey(up => up.Id);

			// 1 User chỉ có 1 Plan tại 1 thời điểm → UNIQUE
			builder.HasIndex(up => up.UserId).IsUnique();

			builder.Property(up => up.StartDate)
				.IsRequired();

			builder.Property(up => up.CurrentClicks)
				.HasDefaultValue(0);

			builder.Property(up => up.CurrentConversions)
				.HasDefaultValue(0);

			builder.Property(up => up.CurrentCampaigns)
				.HasDefaultValue(0);

			builder.HasOne(up => up.User)
				 .WithOne(p => p.UserPlan)
				 .HasForeignKey<UserPlan>(up => up.UserId)
				 .OnDelete(DeleteBehavior.Cascade);

			// Liên kết N–1 với Plan
			builder.HasOne(up => up.Plan)
				.WithMany(p => p.UserPlans)
				.HasForeignKey(up => up.PlanId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
