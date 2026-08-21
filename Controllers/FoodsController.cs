using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Helpers;
using RestaurantManagement.API.Interfaces;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;
    private readonly RestaurantDbContext _context;

    public FoodsController(IFoodService foodService, RestaurantDbContext context)
    {
        _foodService = foodService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetFoods([FromQuery] FoodParams foodParams)
    {
        var pagedFoods = await _foodService.GetFoodsAsync(foodParams);
        var paginationMetadata = new { pagedFoods.TotalCount, pagedFoods.PageSize, pagedFoods.CurrentPage, pagedFoods.TotalPages };
        Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));
        return Ok(pagedFoods);
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeleted()
    {
        var deletedFoods = await _context.Foods
            .IgnoreQueryFilters()
            .Where(f => f.IsDeleted)
            .Include(f => f.Category)
            .Select(f => new FoodDto
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                ImageUrl = f.ImageUrl,
                IsAvailable = f.IsAvailable,
                CategoryId = f.CategoryId,
                CategoryName = f.Category.Name
            })
            .ToListAsync();
        return Ok(deletedFoods);
    }

    [HttpPut("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var food = await _context.Foods.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == id);
        if (food == null) return NotFound();
        food.IsDeleted = false;
        food.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) { var food = await _foodService.GetFoodByIdAsync(id); return food == null ? NotFound() : Ok(food); }
    [HttpPost] public async Task<IActionResult> Create([FromForm] CreateFoodRequest request) { var newFood = await _foodService.CreateFoodAsync(request); return CreatedAtAction(nameof(GetById), new { id = newFood.Id }, newFood); }
    [HttpPut("{id}")] public async Task<IActionResult> Update(int id, [FromForm] UpdateFoodRequest request) { await _foodService.UpdateFoodAsync(id, request); return NoContent(); }
    [HttpDelete("{id}")] public async Task<IActionResult> Delete(int id) { await _foodService.DeleteFoodAsync(id); return NoContent(); }
}