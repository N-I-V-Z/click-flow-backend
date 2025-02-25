using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id).UseIdentityColumn();
            builder.Property(f => f.Description).IsRequired().HasMaxLength(255).IsUnicode();
            builder.Property(f => f.StarRate).IsRequired();
            builder.Property(f => f.Timestamp).IsRequired();

            builder.HasOne(c => c.Campaign).WithMany(f => f.Feedbacks).HasForeignKey(f => f.CampaignId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(u => u.Feedbacker).WithMany(f => f.Feedbacks).HasForeignKey(f => f.FeedbackerId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
