﻿using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Configuration;
using HotelBookingMVC.Finalproject2.Configurations;
using HotelBookingMVC.Finalproject2.Data.Configurations;

namespace HotelBookingMVC.Finalproject2.Data
{
    public class HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : DbContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connectionString = "Server=LAPTOP-76U18161; Database=HotelBookingDb3Context; User Id=sa; password=P@ssword12345; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            var connectionString = "Server=DESKTOP-8FB6QA6; Database=HotelBookingDb3Context; User Id=ADMIN; password=P@ssword12345; TrustServerCertificate=True; Trusted_Connection=False; MultipleActiveResultSets=true;";
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<Hotel> Hotels { get; set; } // Add this line
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<RoomMediaDetail> RoomMediaDetails { get; set; }
        public DbSet<HotelMediaDetail> HotelMediaDetails { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }



        // Add other DbSets as needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new BillConfiguration());
            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            modelBuilder.ApplyConfiguration(new HotelConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new RoomMediaDetailConfiguration());
            modelBuilder.ApplyConfiguration(new HotelMediaDetailConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());



            // Apply other configurations as needed
        }
    }
}
