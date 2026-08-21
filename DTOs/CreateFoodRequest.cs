using Microsoft.AspNetCore.Http;

namespace RestaurantManagement.API.DTOs;

public class CreateFoodRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public IFormFile? Image { get; set; }
}