using AutoMapper;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Entities;
namespace RestaurantManagement.API.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Food, FoodDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<CreateFoodRequest, Food>();
        CreateMap<UpdateFoodRequest, Food>();
        CreateMap<Order, OrderDto>().ForMember(dest => dest.TableName, opt => opt.MapFrom(src => src.Table.Name)).ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<OrderDetail, OrderDetailDto>().ForMember(dest => dest.FoodName, opt => opt.MapFrom(src => src.Food.Name)).ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}