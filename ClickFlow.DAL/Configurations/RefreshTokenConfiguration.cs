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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id)
                   .HasDefaultValueSql("NEWID()"); // Dành cho Guid

            builder.Property(rt => rt.JwtId)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(rt => rt.Token)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(rt => rt.IsUsed)
                   .IsRequired();
            builder.Property(rt => rt.IsRevoked)
                   .IsRequired();
            builder.Property(rt => rt.IssuedAt)
                   .IsRequired();
            builder.Property(rt => rt.ExpiredAt)
                   .IsRequired();

            builder.HasOne(rt => rt.ApplicationUser)
                   .WithMany() // Nếu ApplicationUser không có collection RefreshTokens
                   .HasForeignKey(rt => rt.UserId)
                   .IsRequired();
        }
    }
}
