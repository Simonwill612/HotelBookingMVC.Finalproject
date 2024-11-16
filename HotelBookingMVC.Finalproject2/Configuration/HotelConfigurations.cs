﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasKey(h => h.HotelID);
            builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
            builder.Property(h => h.Address).IsRequired().HasMaxLength(500);
            builder.Property(h => h.City).IsRequired().HasMaxLength(100);
            builder.Property(h => h.State).IsRequired().HasMaxLength(100);
            builder.Property(h => h.ZipCode).IsRequired().HasMaxLength(20);
            builder.Property(h => h.PhoneNumber).HasMaxLength(20);
            builder.Property(h => h.Email).HasMaxLength(100);
            builder.Property(h => h.Description).HasMaxLength(1000);
            builder.Property(h => h.CreatedAt).IsRequired();
            builder.Property(h => h.UpdatedAt).IsRequired();
            builder.HasMany(h => h.Promotions)
                    .WithOne(p => p.Hotel) 
                    .HasForeignKey(p => p.HotelID) 
                    .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Hotels");
        }
    }
}
