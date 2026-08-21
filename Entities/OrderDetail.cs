namespace RestaurantManagement.API.Entities;
public class OrderDetail : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int FoodId { get; set; }
    public Food Food { get; set; } = null!;
}