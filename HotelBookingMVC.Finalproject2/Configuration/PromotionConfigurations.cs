using HotelBookingMVC.Finalproject2.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(p => p.PromotionID);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(50);
        builder.Property(p => p.DiscountAmount).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.ExpirationDate).IsRequired();
        builder.Property(p => p.QuantityLimit).IsRequired();
        //builder.Property(p => p.UsedCount).IsRequired().HasDefaultValue(0);

        builder.HasOne(p => p.Hotel)
       .WithMany(h => h.Promotions)
       .HasForeignKey(p => p.HotelID)
       .OnDelete(DeleteBehavior.Restrict);

    }
}
