using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickFlow.DAL.Entities;

namespace ClickFlow.DAL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(tr => tr.Id);
            builder.Property(tr => tr.Id).UseIdentityColumn();

            builder.Property(tr => tr.Amount)
                   .IsRequired();
            builder.Property(tr => tr.PaymentDate)
                   .IsRequired();
            builder.Property(tr => tr.Status)
                   .IsRequired(false);
            builder.Property(tr => tr.Balance)
                   .IsRequired(false);
            builder.Property(tr => tr.TransactionType)
                   .IsRequired();

            builder.HasOne(tr => tr.Wallet)
                   .WithMany(w => w.Transactions)
                   .HasForeignKey(tr => tr.WalletId)
                   .IsRequired(false);
        }
    }

}
