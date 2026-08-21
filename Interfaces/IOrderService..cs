using RestaurantManagement.API.DTOs;
namespace RestaurantManagement.API.Interfaces;
public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetActiveOrdersAsync();
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
    Task UpdateOrderStatusAsync(int orderId, int status);
}