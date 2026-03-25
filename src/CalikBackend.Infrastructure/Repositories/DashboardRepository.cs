using CalikBackend.Application.DTOs.Dashboard;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _db;

    public DashboardRepository(AppDbContext db) => _db = db;

    public async Task<DashboardResponse> GetDashboardAsync(DateTime from, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // Stat cards
        var totalProducts  = await _db.Products.CountAsync(ct);
        var activeProducts = await _db.Products.CountAsync(p => p.IsActive, ct);
        var totalStock     = await _db.Products.SumAsync(p => p.Stock, ct);
        var receipts7d     = await _db.Invoices.CountAsync(i => i.CreatedAt >= from, ct);

        // Build last 7 days list (oldest → newest)
        var days = Enumerable.Range(0, 7)
            .Select(i => now.AddDays(-(6 - i)).Date)
            .ToList();

        // Fetch raw items and group client-side to avoid Npgsql GroupBy(Date) translation issues
        var recentItems = await _db.InvoiceItems
            .Include(ii => ii.Invoice)
            .Where(ii => ii.Invoice.CreatedAt >= from)
            .Select(ii => new { ii.Invoice.CreatedAt, ii.Quantity })
            .ToListAsync(ct);

        var recentInvoices = await _db.Invoices
            .Where(i => i.CreatedAt >= from)
            .Select(i => new { i.CreatedAt, i.TotalAmount })
            .ToListAsync(ct);

        var unitsByDay = recentItems
            .GroupBy(ii => ii.CreatedAt.Date)
            .ToDictionary(g => g.Key, g => g.Sum(ii => ii.Quantity));

        var revenueByDay = recentInvoices
            .GroupBy(i => i.CreatedAt.Date)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

        var invoiceCountByDay = recentInvoices
            .GroupBy(i => i.CreatedAt.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        // Top 5 products by total units sold (all time) - group client-side
        var allItems = await _db.InvoiceItems
            .Select(ii => new { ii.ProductId, ii.ProductName, ii.Unit, ii.Quantity })
            .ToListAsync(ct);

        var topProducts = allItems
            .GroupBy(ii => new { ii.ProductId, ii.ProductName, ii.Unit })
            .Select(g => new TopProduct(g.Key.ProductId, g.Key.ProductName, g.Key.Unit, g.Sum(ii => ii.Quantity)))
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToList();

        // Invoice status breakdown
        var statusCounts = await _db.Invoices
            .GroupBy(i => i.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Map days to series (fill 0 for missing days)
        var unitsSeries = days.Select(d => new DailyUnitsSold(
            d.ToString("ddd"), DateOnly.FromDateTime(d),
            unitsByDay.GetValueOrDefault(d, 0))).ToList();

        var revenueSeries = days.Select(d => new DailyRevenue(
            d.ToString("ddd"), DateOnly.FromDateTime(d),
            revenueByDay.GetValueOrDefault(d, 0))).ToList();

        var countSeries = days.Select(d => new DailyInvoiceCount(
            d.ToString("ddd"), DateOnly.FromDateTime(d),
            invoiceCountByDay.GetValueOrDefault(d, 0))).ToList();

        var statusSeries = statusCounts
            .Select(s => new InvoiceStatusCount(s.Status.ToString(), s.Count))
            .ToList();

        return new DashboardResponse
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            TotalStock = totalStock,
            Receipts7d = receipts7d,
            IncomingShipments = null,
            UnitsSoldPerDay = unitsSeries,
            RevenuePerDay = revenueSeries,
            InvoiceCountPerDay = countSeries,
            TopProductsBySales = topProducts,
            InvoicesByStatus = statusSeries
        };
    }
}
