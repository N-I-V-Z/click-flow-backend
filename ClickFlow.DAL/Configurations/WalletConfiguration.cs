using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
				   .HasForeignKey<Wallet>(u => u.UserId)
				   .IsRequired(false);

			builder.HasMany(w => w.Transactions)
				   .WithOne(t => t.Wallet)
				   .HasForeignKey(t => t.WalletId)
				   .IsRequired(false);
		}
	}
}
