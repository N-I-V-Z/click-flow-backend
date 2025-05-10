using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class CourseConfiguration : IEntityTypeConfiguration<Course>
	{
		public void Configure(EntityTypeBuilder<Course> builder)
		{
			builder.ToTable("Courses");
			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).UseIdentityColumn();

			builder.Property(c => c.Title).IsRequired().HasMaxLength(255);
			builder.Property(c => c.CreateAt).IsRequired(false);
			builder.Property(c => c.UpdateAt).IsRequired(false);
			builder.Property(c => c.CreateById).IsRequired();
			builder.Property(c => c.Price).IsRequired();
			builder.Property(c => c.AvgRate).IsRequired(false);
			builder.Property(c => c.LessonLearned).IsRequired(false);
			builder.Property(c => c.Content).IsRequired(false);
			builder.Property(c => c.Description).IsRequired(false);
		}
	}
}
