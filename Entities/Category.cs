namespace RestaurantManagement.API.Entities;
public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<Food> Foods { get; set; } = new List<Food>();
}