using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Helpers;
using RestaurantManagement.API.Interfaces;
namespace RestaurantManagement.API.Repositories;
public class FoodRepository : IFoodRepository
{
    private readonly RestaurantDbContext _context;
    public FoodRepository(RestaurantDbContext context) { _context = context; }
    public async Task<PagedList<Food>> GetFoodsAsync(FoodParams foodParams)
    {
        var query = _context.Foods.Include(f => f.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(foodParams.SearchTerm))
            query = query.Where(f => f.Name.ToLower().Contains(foodParams.SearchTerm.ToLower()));
        if (foodParams.CategoryId.HasValue)
            query = query.Where(f => f.CategoryId == foodParams.CategoryId);
        query = foodParams.OrderBy switch
        {
            "price_desc" => query.OrderByDescending(f => f.Price),
            "price_asc" => query.OrderBy(f => f.Price),
            "name_asc" => query.OrderBy(f => f.Name),
            "name_desc" => query.OrderByDescending(f => f.Name),
            _ => query.OrderByDescending(f => f.CreatedAt)
        };
        return await PagedList<Food>.CreateAsync(query, foodParams.PageNumber, foodParams.PageSize);
    }
    public async Task<Food?> GetByIdAsync(int id) => await _context.Foods.Include(f => f.Category).FirstOrDefaultAsync(f => f.Id == id);
    public async Task<Food> AddAsync(Food food) { _context.Foods.Add(food); await _context.SaveChangesAsync(); return food; }
    public async Task UpdateAsync(Food food) { _context.Foods.Update(food); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(Food food) { _context.Foods.Remove(food); await _context.SaveChangesAsync(); }
}