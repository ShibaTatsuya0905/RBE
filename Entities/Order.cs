namespace RestaurantManagement.API.Entities;
public class Order : BaseEntity
{
    public string OrderCode { get; set; } = null!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public int TableId { get; set; }
    public Table Table { get; set; } = null!;
    public int? CashierId { get; set; }
    public User? Cashier { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}