namespace RestaurantManagement.API.DTOs;
public class OrderItemRequest
{
    public int FoodId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}