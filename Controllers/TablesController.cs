using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.Entities;
using RestaurantManagement.API.Hubs;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TablesController : ControllerBase
{
    private readonly RestaurantDbContext _context;
    private readonly IHubContext<OrderHub> _hubContext;

    public TablesController(RestaurantDbContext context, IHubContext<OrderHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tables = await _context.Tables.ToListAsync();
        return Ok(tables);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return NotFound();
        return Ok(table);
    }

    [HttpPost("{id}/call")]
    public async Task<IActionResult> CallWaiter(int id, [FromBody] string type)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return NotFound();

        await _hubContext.Clients.All.SendAsync("ReceiveTableCall", id, table.Name, type);
        return Ok();
    }

    public class CreateTableDto { public string Name { get; set; } = null!; public int Capacity { get; set; } }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableDto request)
    {
        var table = new Table { Name = request.Name, Capacity = request.Capacity, Status = TableStatus.Available };
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();
        return Ok(table);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return NotFound();

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}