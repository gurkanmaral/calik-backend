namespace CalikBackend.Application.DTOs.Customers;

public class AdjustBalanceRequest
{
    public decimal Amount { get; set; }  // positive or negative adjustment
}
