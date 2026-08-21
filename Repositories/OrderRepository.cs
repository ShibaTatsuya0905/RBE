using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Interfaces;
namespace RestaurantManagement.API.Repositories;
public class OrderRepository : IOrderRepository
{
    private readonly RestaurantDbContext _context;
    public OrderRepository(RestaurantDbContext context) { _context = context; }
    public async Task<IEnumerable<Order>> GetActiveOrdersAsync() => await _context.Orders.Include(o => o.Table).Include(o => o.OrderDetails).ThenInclude(od => od.Food).Where(o => o.Status != OrderStatus.Paid && o.Status != OrderStatus.Cancelled).OrderByDescending(o => o.CreatedAt).ToListAsync();
    public async Task<Order?> GetByIdAsync(int id) => await _context.Orders.Include(o => o.Table).Include(o => o.OrderDetails).ThenInclude(od => od.Food).FirstOrDefaultAsync(o => o.Id == id);
    public async Task<Order> CreateAsync(Order order) { _context.Orders.Add(order); await _context.SaveChangesAsync(); return order; }
    public async Task UpdateAsync(Order order) { _context.Orders.Update(order); await _context.SaveChangesAsync(); }
}