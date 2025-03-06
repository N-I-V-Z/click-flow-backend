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
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id).UseIdentityColumn();

            builder.Property(w => w.Balance)
                   .IsRequired();

            builder.HasOne(w => w.ApplicationUser)
                   .WithOne(u => u.Wallet)
                   .HasForeignKey<ApplicationUser>(u => u.WalletId)
                   .IsRequired(false);

            builder.HasMany(w => w.Transactions)
                   .WithOne(t => t.Wallet)
                   .HasForeignKey(t => t.WalletId)
                   .IsRequired(false);
        }
    }
}
