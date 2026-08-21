namespace RestaurantManagement.API.DTOs;

public class UpdateFoodRequest : CreateFoodRequest
{
    public bool IsAvailable { get; set; }
}