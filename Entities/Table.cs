using CloudinaryDotNet.Actions;

namespace RestaurantManagement.API.Entities;
public class Table : BaseEntity
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string? QrCodeUrl { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}