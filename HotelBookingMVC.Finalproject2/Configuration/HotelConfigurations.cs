using HotelBookingMVC.Finalproject2.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasKey(h => h.HotelID);

        builder.Property(h => h.UserID)
            .IsRequired(); // Đảm bảo UserID không được để trống

        //builder.HasIndex(h => h.UserID)
        //    .IsUnique();

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(h => h.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(h => h.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(h => h.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(h => h.Email)
            .HasMaxLength(100);

        builder.Property(h => h.Description)
            .HasMaxLength(1000);

        builder.Property(h => h.CreatedAt)
            .IsRequired();

        builder.Property(h => h.UpdatedAt)
            .IsRequired();

        builder.HasMany(h => h.Promotions)
            .WithOne(p => p.Hotel)
            .HasForeignKey(p => p.HotelID)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa các khuyến mãi khi khách sạn bị xóa

        builder.HasMany(h => h.Feedbacks) // Thiết lập mối quan hệ với Feedback
            .WithOne(f => f.Hotel)
            .HasForeignKey(f => f.HotelID)
            .OnDelete(DeleteBehavior.Cascade); // Xóa phản hồi khi khách sạn bị xóa

        builder.ToTable("Hotels");
    }
}
