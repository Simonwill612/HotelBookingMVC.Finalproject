using HotelBookingMVC.Finalproject2.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingMVC.Finalproject2.Data.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            // Khóa chính
            builder.HasKey(f => f.FeedbackID);

            // Cấu hình các thuộc tính
            builder.Property(f => f.StarRating)
                .IsRequired(); // Bắt buộc

            builder.Property(f => f.StarRating)
                .IsRequired();

            builder.HasCheckConstraint("CK_Feedback_StarRating", "StarRating >= 1 AND StarRating <= 5");


            builder.Property(f => f.Comment)
                .HasMaxLength(1000); // Giới hạn chiều dài bình luận

            builder.Property(f => f.DateCreated)
                .HasDefaultValueSql("GETDATE()"); // Giá trị mặc định là ngày giờ hiện tại

            // Thiết lập mối quan hệ với Hotel
            builder.HasOne(f => f.Hotel)
                .WithMany(h => h.Feedbacks)
                .HasForeignKey(f => f.HotelID)
                .OnDelete(DeleteBehavior.Cascade); // Xóa phản hồi khi khách sạn bị xóa
        }
    }
}