namespace RestaurantManagement.API.DTOs;
public class OrderDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TableId { get; set; }
    public string TableName { get; set; } = null!;
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}