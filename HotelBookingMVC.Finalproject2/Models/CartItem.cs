public class CartItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CheckInDate { get; set; } // Add this property
    public DateTime CheckOutDate { get; set; } // Add this property
}
