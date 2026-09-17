using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.API.DTOs;
using RestaurantManagement.API.Interfaces;

namespace RestaurantManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IConfiguration _config;

    public PaymentsController(IOrderService orderService, IConfiguration config)
    {
        _orderService = orderService;
        _config = config;
    }

    [HttpPost("sepay-webhook")]
    public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookDto payload)
    {
        var apiKeyConfig = _config["SePay:ApiKey"];
        if (!string.IsNullOrEmpty(apiKeyConfig))
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != $"Apikey {apiKeyConfig}")
            {
                return Unauthorized(new { message = "Invalid API Key" });
            }
        }

        if (payload.TransferType != "in")
        {
            return Ok(new { success = true, message = "Ignored outgoing transfer" });
        }

        var success = await _orderService.ProcessPaymentWebhookAsync(payload.Content, payload.TransferAmount);

        if (success)
        {
            return Ok(new { success = true, message = "Payment processed successfully" });
        }

        return Ok(new { success = false, message = "Order not found or insufficient amount" });
    }
}