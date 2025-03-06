using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).UseIdentityColumn();

            builder.Property(f => f.Description)
                   .IsRequired();
            builder.Property(f => f.StarRate)
                   .IsRequired();
            builder.Property(f => f.Timestamp)
                   .IsRequired();

            builder.HasOne(f => f.Campaign)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(f => f.CampaignId)
                   .IsRequired(false);
            builder.HasOne(f => f.Feedbacker)
                   .WithMany(p => p.Feedbacks)
                   .HasForeignKey(f => f.FeedbackerId)
                   .IsRequired(false);
        }
    }

}
