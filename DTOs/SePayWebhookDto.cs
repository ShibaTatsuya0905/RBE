namespace RestaurantManagement.API.DTOs;

public class SePayWebhookDto
{
    public long Id { get; set; }
    public string Gateway { get; set; } = null!;           
    public string TransactionDate { get; set; } = null!;
    public string AccountNumber { get; set; } = null!;      
    public string Content { get; set; } = null!;           
    public string TransferType { get; set; } = null!;       
    public decimal TransferAmount { get; set; }             
    public string? ReferenceCode { get; set; }
}