using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClickFlow.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).UseIdentityColumn();
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(255).IsUnicode();
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.Gender).IsRequired();
            builder.Property(u => u.Password).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(255);

            builder.HasOne(p => p.Publisher).WithOne(u => u.User).HasForeignKey<User>(u => u.PublisherId);
            builder.HasOne(p => p.Advertiser).WithOne(u => u.User).HasForeignKey<User>(u => u.AdvertiserId);
            builder.HasOne(w => w.Wallet).WithOne(u => u.User).HasForeignKey<User>(u => u.WalletId);
        }
    }
}
