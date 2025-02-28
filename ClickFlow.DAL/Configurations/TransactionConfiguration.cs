using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).UseIdentityColumn();
            builder.Property(t => t.Amount).IsRequired();
            builder.Property(t => t.PaymentDate).IsRequired();
            builder.Property(t => t.Status).IsRequired(false);
            builder.Property(t => t.Balance).IsRequired(false);
            builder.Property(t => t.TransactionType).IsRequired();

            builder.HasOne(w => w.Wallet).WithMany(t => t.Transactions).HasForeignKey(t => t.WalletId);
        }
    }
}
