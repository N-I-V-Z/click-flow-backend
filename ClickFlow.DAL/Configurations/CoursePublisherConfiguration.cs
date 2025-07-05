using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class CoursePublisherConfiguration : IEntityTypeConfiguration<CoursePublisher>
	{
		public void Configure(EntityTypeBuilder<CoursePublisher> builder)
		{
			builder.ToTable("CoursePublishers");
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).UseIdentityColumn();

			builder.Property(c => c.PublisherId).IsRequired();
			builder.Property(c => c.CourseId).IsRequired();
			builder.Property(c => c.Rate).IsRequired(false);

			builder.HasOne(c => c.Publisher)
				   .WithMany(p => p.CoursePublishers)
				   .HasForeignKey(c => c.PublisherId)
				   .IsRequired(false);

			builder.HasOne(cp => cp.Course)
				   .WithMany(c => c.CoursePublishers)
				   .HasForeignKey(cp => cp.CourseId)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
