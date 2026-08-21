using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Hubs;
using RestaurantManagement.API.Interfaces;

namespace RestaurantManagement.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IFoodRepository _foodRepo;
    private readonly RestaurantDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderService(IOrderRepository orderRepo, IFoodRepository foodRepo, RestaurantDbContext context, IMapper mapper, IHubContext<OrderHub> hubContext)
    {
        _orderRepo = orderRepo;
        _foodRepo = foodRepo;
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<IEnumerable<OrderDto>> GetActiveOrdersAsync() => _mapper.Map<IEnumerable<OrderDto>>(await _orderRepo.GetActiveOrdersAsync());

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
    {
        var newOrder = new Order { TableId = request.TableId, OrderCode = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}", Status = OrderStatus.Pending, CreatedAt = DateTime.UtcNow };
        decimal totalAmount = 0;
        foreach (var item in request.Items)
        {
            var food = await _foodRepo.GetByIdAsync(item.FoodId);
            if (food != null && food.IsAvailable)
            {
                newOrder.OrderDetails.Add(new OrderDetail { FoodId = food.Id, Quantity = item.Quantity, UnitPrice = food.Price, Notes = item.Notes, Status = OrderStatus.Pending });
                totalAmount += food.Price * item.Quantity;
            }
        }
        newOrder.TotalAmount = totalAmount;

        var table = await _context.Tables.FindAsync(request.TableId);
        if (table != null)
        {
            table.Status = TableStatus.Occupied;
            _context.Tables.Update(table);
        }

        var createdOrder = await _orderRepo.CreateAsync(newOrder);
        var orderDto = _mapper.Map<OrderDto>(await _orderRepo.GetByIdAsync(createdOrder.Id));

        await _hubContext.Clients.All.SendAsync("ReceiveNewOrder", orderDto);
        await _hubContext.Clients.All.SendAsync("TableStatusUpdated", request.TableId, 1);

        return orderDto;
    }

    public async Task UpdateOrderStatusAsync(int orderId, int statusInt)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = (OrderStatus)statusInt;
            await _orderRepo.UpdateAsync(order);

            if (order.Status == OrderStatus.Paid)
            {
                var table = await _context.Tables.FindAsync(order.TableId);
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                    _context.Tables.Update(table);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("TableStatusUpdated", table.Id, 0);
                }
            }

            await _hubContext.Clients.All.SendAsync("OrderStatusUpdated", orderId, order.Status.ToString());
        }
    }
}