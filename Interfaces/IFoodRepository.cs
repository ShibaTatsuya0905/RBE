using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Helpers;
namespace RestaurantManagement.API.Interfaces;
public interface IFoodRepository
{
    Task<PagedList<Food>> GetFoodsAsync(FoodParams foodParams);
    Task<Food?> GetByIdAsync(int id);
    Task<Food> AddAsync(Food food);
    Task UpdateAsync(Food food);
    Task DeleteAsync(Food food);
}