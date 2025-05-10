using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	internal class QuestionAndAnswerConfiguration : IEntityTypeConfiguration<QuestionAndAnswer>
	{
		public void Configure(EntityTypeBuilder<QuestionAndAnswer> builder)
		{
			builder.ToTable("QuestionAndAnswers");
			builder.HasKey(q => q.Id);
			builder.Property(q => q.Id).UseIdentityColumn();

			builder.Property(q => q.Question).IsRequired().HasMaxLength(255);
			builder.Property(q => q.Answer).IsRequired(false).HasMaxLength(255);
			builder.Property(q => q.Timestamp).IsRequired(false);

			builder.HasOne(q => q.Video)
				   .WithMany(v => v.QuestionAndAnswers)
				   .HasForeignKey(q => q.VideoId)
				   .IsRequired(false);
			builder.HasOne(q => q.Publisher)
				   .WithMany(p => p.QuestionAndAnswers)
				   .HasForeignKey(q => q.PublisherId)
				   .IsRequired(false);
		}
	}
}
