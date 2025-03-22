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
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.ToTable("Publishers");
            builder.HasKey(p => p.UserId);           
            builder.Property(p => p.UserId)
                   .ValueGeneratedNever();
            builder.HasOne(u => u.ApplicationUser)
                   .WithOne(p => p.Publisher)
                   .HasForeignKey<Publisher>(p => p.UserId);

            builder.HasMany(p => p.Reports)
                   .WithOne(r => r.Publisher)
                   .HasForeignKey(r => r.PublisherId)
                   .IsRequired(false);
            builder.HasMany(p => p.Feedbacks)
                   .WithOne(f => f.Feedbacker)
                   .HasForeignKey(f => f.FeedbackerId)
                   .IsRequired(false);
        }
    }

}
