using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class VideoConfiguration : IEntityTypeConfiguration<Video>
	{
		public void Configure(EntityTypeBuilder<Video> builder)
		{
			builder.ToTable("Videos");
			builder.HasKey(v => v.Id);
			builder.Property(v => v.Id).UseIdentityColumn();

			builder.Property(v => v.Link).IsRequired();
			builder.Property(v => v.Title).IsRequired();

			builder.HasOne(v => v.Course)
				   .WithMany(c => c.Videos)
				   .HasForeignKey(v => v.CourseId)
				   .IsRequired(false);
		}
	}
}
