using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class CommentConfiguration : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.ToTable("Comments");

			builder.HasKey(c => c.Id);

			builder.Property(c => c.Content)
				.IsRequired()
				.HasMaxLength(1000);

			builder.Property(c => c.CreatedAt)
				.IsRequired();

			builder.HasOne(c => c.Post)
				.WithMany(p => p.Comments)
				.HasForeignKey(c => c.PostId)
				 .OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(c => c.ParentComment)
				.WithMany(c => c.Replies)
				.HasForeignKey(c => c.ParentCommentId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasOne(c => c.Author)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.AuthorId)
				 .OnDelete(DeleteBehavior.Restrict);

			builder.Property(c => c.IsDeleted)
				.IsRequired()
				.HasDefaultValue(false);

		}
	}
}
