using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Interfaces;
namespace RestaurantManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService) { _orderService = orderService; }
    [HttpGet("active")] public async Task<IActionResult> GetActiveOrders() => Ok(await _orderService.GetActiveOrdersAsync());
    [HttpPost] public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request) => Ok(await _orderService.CreateOrderAsync(request));
    [HttpPut("{id}/status")] public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status) { await _orderService.UpdateOrderStatusAsync(id, status); return Ok(); }
    [HttpPut("{id}/pay")] public async Task<IActionResult> PayOrder(int id, [FromBody] string paymentMethod) { await _orderService.UpdateOrderStatusAsync(id, 4); return Ok(); }
}