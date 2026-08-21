using AutoMapper;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Helpers;
using RestaurantManagement.API.Interfaces;
using RestaurantManagement.API.ExternalServices;

namespace RestaurantManagement.API.Services;

public class FoodService : IFoodService
{
    private readonly IFoodRepository _foodRepository;
    private readonly IPhotoService _photoService;
    private readonly IMapper _mapper;

    public FoodService(IFoodRepository foodRepository, IPhotoService photoService, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _photoService = photoService;
        _mapper = mapper;
    }

    public async Task<PagedList<FoodDto>> GetFoodsAsync(FoodParams foodParams)
    {
        var foods = await _foodRepository.GetFoodsAsync(foodParams);
        var foodDtos = _mapper.Map<List<FoodDto>>(foods);
        return new PagedList<FoodDto>(foodDtos, foods.TotalCount, foods.CurrentPage, foods.PageSize);
    }

    public async Task<FoodDto?> GetFoodByIdAsync(int id)
    {
        var food = await _foodRepository.GetByIdAsync(id);
        return food == null ? null : _mapper.Map<FoodDto>(food);
    }

    public async Task<FoodDto> CreateFoodAsync(CreateFoodRequest request)
    {
        var foodEntity = _mapper.Map<Food>(request);
        if (request.Image != null)
        {
            foodEntity.ImageUrl = await _photoService.AddPhotoAsync(request.Image);
        }
        var createdFood = await _foodRepository.AddAsync(foodEntity);
        return _mapper.Map<FoodDto>(createdFood);
    }

    public async Task UpdateFoodAsync(int id, UpdateFoodRequest request)
    {
        var food = await _foodRepository.GetByIdAsync(id);
        if (food != null)
        {
            _mapper.Map(request, food);
            if (request.Image != null)
            {
                food.ImageUrl = await _photoService.AddPhotoAsync(request.Image);
            }
            await _foodRepository.UpdateAsync(food);
        }
    }

    public async Task DeleteFoodAsync(int id)
    {
        var food = await _foodRepository.GetByIdAsync(id);
        if (food != null) await _foodRepository.DeleteAsync(food);
    }
}