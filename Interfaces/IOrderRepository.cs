using RestaurantManagement.API.Entities;
namespace RestaurantManagement.API.Interfaces;
public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetActiveOrdersAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<Order> CreateAsync(Order order);
    Task UpdateAsync(Order order);
}