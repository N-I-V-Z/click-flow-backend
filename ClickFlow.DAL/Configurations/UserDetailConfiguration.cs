﻿using ClickFlow.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Configurations
{
    public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
    {
        public void Configure(EntityTypeBuilder<UserDetail> builder)
        {
            builder.ToTable("UserDetails");
            builder.HasKey(ud => ud.Id);
            builder.Property(ud => ud.Id).UseIdentityColumn();

            builder.Property(ud => ud.DateOfBirth)
                   .IsRequired(false);
            builder.Property(ud => ud.Gender)
                   .IsRequired();
            builder.Property(ud => ud.AvatarURL)
                   .HasMaxLength(500);
            builder.Property(ud => ud.Address)
                   .HasMaxLength(500);
            builder.Property(ud => ud.IdentityCard)
                   .HasMaxLength(100);

            builder.HasOne(ud => ud.User)
                   .WithOne() // Nếu ApplicationUser không có navigation property cho UserDetail
                   .HasForeignKey<UserDetail>(ud => ud.ApplicationUserId)
                   .IsRequired();
        }
    }
}
