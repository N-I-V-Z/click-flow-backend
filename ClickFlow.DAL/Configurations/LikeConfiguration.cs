using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class LikeConfiguration : IEntityTypeConfiguration<Like>
	{
		public void Configure(EntityTypeBuilder<Like> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.CreatedAt)
				.IsRequired();

			builder.Property(x => x.IsDeleted)
				.HasDefaultValue(false);

			builder.HasOne(x => x.Post)
				.WithMany(x => x.Likes)
				.HasForeignKey(x => x.PostId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.User)
				.WithMany()
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Đảm bảo mỗi user chỉ like một post một lần
			builder.HasIndex(x => new { x.PostId, x.UserId })
				.IsUnique();
		}
	}
} 