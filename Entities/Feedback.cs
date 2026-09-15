namespace RestaurantManagement.API.Entities;

public class Feedback : BaseEntity
{
    public int TableId { get; set; }
    public int FoodRating { get; set; }
    public int ServiceRating { get; set; }
    public int SpeedRating { get; set; }
    public int ValueRating { get; set; }
    public string? Comment { get; set; }
}