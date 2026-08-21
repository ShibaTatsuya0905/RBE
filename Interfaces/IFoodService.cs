using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Helpers;
namespace RestaurantManagement.API.Interfaces;
public interface IFoodService
{
    Task<PagedList<FoodDto>> GetFoodsAsync(FoodParams foodParams);
    Task<FoodDto?> GetFoodByIdAsync(int id);
    Task<FoodDto> CreateFoodAsync(CreateFoodRequest request);
    Task UpdateFoodAsync(int id, UpdateFoodRequest request);
    Task DeleteFoodAsync(int id);
}