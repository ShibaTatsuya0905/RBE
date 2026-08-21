namespace RestaurantManagement.API.DTOs;
public class CreateOrderRequest
{
    public int TableId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}