﻿using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
			builder.HasOne(u => u.UserDetail)
				   .WithOne(ud => ud.User)
				   .HasForeignKey<UserDetail>(ud => ud.ApplicationUserId);
		}
	}
}
