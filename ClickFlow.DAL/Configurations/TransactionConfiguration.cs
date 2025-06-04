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
