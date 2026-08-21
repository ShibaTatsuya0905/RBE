namespace RestaurantManagement.API.DTOs;
public class OrderDetailDto
{
    public int Id { get; set; }
    public int FoodId { get; set; }
    public string FoodName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = null!;
}