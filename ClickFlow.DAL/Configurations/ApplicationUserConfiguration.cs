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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("ApplicationUsers");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).UseIdentityColumn();

            builder.Property(u => u.FullName)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(u => u.Role)
                   .IsRequired();
            builder.Property(u => u.IsDeleted)
                   .IsRequired();
        }
    }
}
