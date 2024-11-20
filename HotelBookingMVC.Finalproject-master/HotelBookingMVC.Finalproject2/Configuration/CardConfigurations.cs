using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(c => c.CardID);

            builder.Property(c => c.NameOnCard)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CardNumber)
                .IsRequired();

            builder.Property(c => c.Expiration)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(c => c.CVV)
                .IsRequired()
                .HasMaxLength(3);
        }
    }
}
