using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
	public class ConversionConfiguration : IEntityTypeConfiguration<Conversion>
	{
		public void Configure(EntityTypeBuilder<Conversion> builder)
		{
			builder.ToTable("Conversions");

			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).UseIdentityColumn();

			builder.Property(c => c.ClickId)
				   .IsRequired()
				   .HasMaxLength(50);

			builder.HasIndex(c => c.ClickId);

			builder.Property(c => c.OrderId)
				   .HasMaxLength(100);

			builder.Property(c => c.Timestamp)
				   .IsRequired();

			builder.Property(c => c.EventType)
				   .HasConversion<string>()
				   .IsRequired();

			builder.Property(c => c.Status)
				   .HasConversion<string>()
				   .IsRequired();

			builder.HasOne(c => c.Click)
				   .WithMany(t => t.Conversions)
				   .HasForeignKey(c => c.ClickId)
				   .HasPrincipalKey(t => t.ClickId)
				   .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
