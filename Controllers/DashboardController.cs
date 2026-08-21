using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.API.Data;
using RestaurantManagement.API.Entities;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly RestaurantDbContext _context;

    public DashboardController(RestaurantDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] string period = "Year")
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var tomorrow = today.AddDays(1);

        var query = _context.Orders.Where(o => o.Status == OrderStatus.Paid);

        decimal periodRevenue = 0;
        var chartData = new List<object>();

        if (period == "Day")
        {
            var dayOrders = await query.Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow).ToListAsync();
            periodRevenue = dayOrders.Sum(o => o.TotalAmount);

            for (int h = 8; h <= 22; h += 2)
            {
                var hourRevenue = dayOrders.Where(o => o.CreatedAt.Hour >= h && o.CreatedAt.Hour < h + 2).Sum(o => o.TotalAmount) / 1000;
                chartData.Add(new { name = $"{h:00}:00", revenue = (double)hourRevenue });
            }
        }
        else if (period == "Week")
        {
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var weekOrders = await query.Where(o => o.CreatedAt >= startOfWeek && o.CreatedAt < startOfWeek.AddDays(7)).ToListAsync();
            periodRevenue = weekOrders.Sum(o => o.TotalAmount);

            var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                var dayRevenue = weekOrders.Where(o => o.CreatedAt.Date == date).Sum(o => o.TotalAmount) / 1000;
                chartData.Add(new { name = days[i], revenue = (double)dayRevenue });
            }
        }
        else if (period == "Month")
        {
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            var monthOrders = await query.Where(o => o.CreatedAt >= startOfMonth && o.CreatedAt < endOfMonth).ToListAsync();
            periodRevenue = monthOrders.Sum(o => o.TotalAmount);

            for (int w = 1; w <= 4; w++)
            {
                var startW = startOfMonth.AddDays((w - 1) * 7);
                var endW = w == 4 ? endOfMonth : startW.AddDays(7);
                var weekRevenue = monthOrders.Where(o => o.CreatedAt >= startW && o.CreatedAt < endW).Sum(o => o.TotalAmount) / 1000;
                chartData.Add(new { name = $"Week {w}", revenue = (double)weekRevenue });
            }
        }
        else if (period == "All")
        {
            var allOrders = await query.ToListAsync();
            periodRevenue = allOrders.Sum(o => o.TotalAmount);

            var years = allOrders.Select(o => o.CreatedAt.Year).Distinct().OrderBy(y => y).ToList();
            if (!years.Any()) years.Add(now.Year);

            foreach (var year in years)
            {
                var yearRevenue = allOrders.Where(o => o.CreatedAt.Year == year).Sum(o => o.TotalAmount) / 1000000;
                chartData.Add(new { name = year.ToString(), revenue = (double)yearRevenue });
            }
        }
        else
        {
            var startOfYear = new DateTime(now.Year, 1, 1);
            var endOfYear = startOfYear.AddYears(1);
            var yearOrders = await query.Where(o => o.CreatedAt >= startOfYear && o.CreatedAt < endOfYear).ToListAsync();
            periodRevenue = yearOrders.Sum(o => o.TotalAmount);

            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            for (int m = 1; m <= 12; m++)
            {
                var monthRevenueValue = yearOrders.Where(o => o.CreatedAt.Month == m).Sum(o => o.TotalAmount) / 1000000;
                chartData.Add(new { name = months[m - 1], revenue = (double)monthRevenueValue });
            }
        }

        var todayOrdersCount = await _context.Orders.Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow).CountAsync();
        var activeTablesCount = await _context.Orders.Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow).Select(o => o.TableId).Distinct().CountAsync();
        var customersCount = activeTablesCount * 3;

        var todayPaidOrders = await query.Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow).ToListAsync();
        var averageBill = todayOrdersCount > 0 ? todayPaidOrders.Sum(o => o.TotalAmount) / todayOrdersCount : 0;

        var orderDetailsList = await _context.OrderDetails
            .Include(od => od.Food).ThenInclude(f => f.Category)
            .Where(od => od.Order.Status == OrderStatus.Paid)
            .ToListAsync();

        var popularFoods = orderDetailsList
            .GroupBy(od => od.Food.Name)
            .Select(g => new { name = g.Key, value = g.Sum(od => od.Quantity) })
            .OrderByDescending(x => x.value)
            .Take(4)
            .ToList();

        var sevenDaysAgo = today.AddDays(-6);
        var weeklyOrdersList = await _context.Orders
            .Where(o => o.CreatedAt >= sevenDaysAgo && o.Status == OrderStatus.Paid)
            .ToListAsync();

        var daysOfWeek = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        var weeklyWorkload = Enumerable.Range(0, 7).Select(i =>
        {
            var date = sevenDaysAgo.AddDays(i);
            var dayName = date.DayOfWeek == DayOfWeek.Sunday ? "Sun" : date.DayOfWeek.ToString().Substring(0, 3);
            var count = weeklyOrdersList.Count(o => o.CreatedAt.Date == date);
            return new { name = dayName, orders = count };
        }).ToArray();

        var hourlyOrdersList = await _context.Orders
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .ToListAsync();

        var kitchenWorkload = Enumerable.Range(8, 15).Select(h => new
        {
            name = $"{h:00}:00",
            load = hourlyOrdersList.Count(o => o.CreatedAt.Hour == h)
        }).ToArray();

        var revenueByCategory = orderDetailsList
            .GroupBy(od => od.Food.Name)
            .Select(g => new { name = g.Key, value = (double)g.Sum(od => od.Quantity * od.UnitPrice) })
            .ToList();

        return Ok(new
        {
            todayRevenue = periodRevenue,
            ordersToday = todayOrdersCount,
            customers = customersCount,
            averageBill,
            monthlyRevenue = chartData,
            popularFoods,
            weeklyWorkload,
            kitchenWorkload,
            revenueByCategory
        });
    }
}