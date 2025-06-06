using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
	{
		public void Configure(EntityTypeBuilder<PaymentMethod> builder)
		{
			builder.ToTable("PaymentMethods");
			builder.HasKey(pm => pm.Id);
			builder.Property(pm => pm.Id).UseIdentityColumn();

			builder.Property(pm => pm.PaymentInfo)
				   .IsRequired()
				   .HasMaxLength(500);
			builder.Property(pm => pm.BankName)
				   .HasMaxLength(200);
			builder.Property(pm => pm.IsDefault)
				   .IsRequired();

			builder.HasOne(pm => pm.User)
				   .WithMany() // Nếu ApplicationUser không có collection PaymentMethods
				   .HasForeignKey(pm => pm.UserId)
				   .IsRequired();
		}
	}
}
