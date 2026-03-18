namespace CalikBackend.Application.DTOs.Dashboard;

public class DashboardResponse
{
    // Stat cards
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalStock { get; set; }
    public int Receipts7d { get; set; }
    public int? IncomingShipments { get; set; }

    // Chart series (last 7 days, index 0 = oldest day)
    public List<DailyUnitsSold> UnitsSoldPerDay { get; set; } = [];
    public List<DailyRevenue> RevenuePerDay { get; set; } = [];
    public List<DailyInvoiceCount> InvoiceCountPerDay { get; set; } = [];
    public List<TopProduct> TopProductsBySales { get; set; } = [];
    public List<InvoiceStatusCount> InvoicesByStatus { get; set; } = [];
}

public record DailyUnitsSold(string Day, DateOnly Date, int Units);
public record DailyRevenue(string Day, DateOnly Date, decimal Revenue);
public record DailyInvoiceCount(string Day, DateOnly Date, int Count);
public record TopProduct(Guid ProductId, string ProductName, string Unit, int TotalSold);
public record InvoiceStatusCount(string Status, int Count);
