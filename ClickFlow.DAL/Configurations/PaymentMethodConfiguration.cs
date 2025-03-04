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
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.PaymentInfo).IsRequired().HasMaxLength(255);
            builder.Property(p => p.BankName).HasMaxLength(100);
            builder.Property(p => p.IsDefault).IsRequired().HasDefaultValue(false);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.PaymentMethods)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
