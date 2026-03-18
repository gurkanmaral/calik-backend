using CalikBackend.Application.DTOs.Dashboard;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _db;

    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _db = db;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);

        // Stat cards
        var totalProducts = await _db.Products.CountAsync();
        var activeProducts = await _db.Products.CountAsync(p => p.IsActive);
        var totalStock = await _db.Products.SumAsync(p => p.Stock);
        var receipts7d = await _db.Invoices.CountAsync(i => i.CreatedAt >= sevenDaysAgo);

        // Build last 7 days list (oldest → newest)
        var days = Enumerable.Range(0, 7)
            .Select(i => now.AddDays(-(6 - i)).Date)
            .ToList();

        // Units sold per day — sum InvoiceItem.Quantity grouped by Invoice.CreatedAt date
        var unitsByDay = await _db.InvoiceItems
            .Include(ii => ii.Invoice)
            .Where(ii => ii.Invoice.CreatedAt >= sevenDaysAgo)
            .GroupBy(ii => ii.Invoice.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Units = g.Sum(ii => ii.Quantity) })
            .ToListAsync();

        // Revenue per day — sum Invoice.TotalAmount grouped by CreatedAt date
        var revenueByDay = await _db.Invoices
            .Where(i => i.CreatedAt >= sevenDaysAgo)
            .GroupBy(i => i.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Revenue = g.Sum(i => i.TotalAmount) })
            .ToListAsync();

        // Invoice count per day
        var invoiceCountByDay = await _db.Invoices
            .Where(i => i.CreatedAt >= sevenDaysAgo)
            .GroupBy(i => i.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        // Top 5 products by total units sold (all time)
        var topProducts = (await _db.InvoiceItems
            .GroupBy(ii => new { ii.ProductId, ii.ProductName, ii.Unit })
            .Select(g => new { g.Key.ProductId, g.Key.ProductName, g.Key.Unit, TotalSold = g.Sum(ii => ii.Quantity) })
            .OrderByDescending(x => x.TotalSold)
            .Take(5)
            .ToListAsync())
            .Select(x => new TopProduct(x.ProductId, x.ProductName, x.Unit, x.TotalSold))
            .ToList();

        // Invoice status breakdown
        var statusCounts = await _db.Invoices
            .GroupBy(i => i.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        // Map days to series (fill 0 for missing days)
        var unitsSeries = days.Select(d => new DailyUnitsSold(
            d.ToString("ddd"), DateOnly.FromDateTime(d),
            unitsByDay.FirstOrDefault(x => x.Date == d)?.Units ?? 0)).ToList();

        var revenueSeries = days.Select(d => new DailyRevenue(
            d.ToString("ddd"), DateOnly.FromDateTime(d),
            revenueByDay.FirstOrDefault(x => x.Date == d)?.Revenue ?? 0)).ToList();

        var countSeries = days.Select(d => new DailyInvoiceCount(
            d.ToString("ddd"), DateOnly.FromDateTime(d),
            invoiceCountByDay.FirstOrDefault(x => x.Date == d)?.Count ?? 0)).ToList();

        var statusSeries = statusCounts
            .Select(s => new InvoiceStatusCount(s.Status.ToString(), s.Count))
            .ToList();

        return Ok(new DashboardResponse
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
        });
    }

    // GET api/admin/roles
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
        return Ok(roles);
    }

    // GET api/admin/users
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new
        {
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.PhoneNumber
        }).ToList();

        return Ok(users);
    }

    // POST api/admin/users/{userId}/roles
    [HttpPost("users/{userId}/roles")]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] RoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        if (!await _roleManager.RoleExistsAsync(request.Role))
            return BadRequest(new { message = $"Role '{request.Role}' does not exist." });

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return BadRequest(new { message = "User already has this role." });

        await _userManager.AddToRoleAsync(user, request.Role);
        return Ok(new { message = $"Role '{request.Role}' assigned to {user.Email}." });
    }

    // DELETE api/admin/users/{userId}/roles
    [HttpDelete("users/{userId}/roles")]
    public async Task<IActionResult> RemoveRole(string userId, [FromBody] RoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        if (!await _userManager.IsInRoleAsync(user, request.Role))
            return BadRequest(new { message = "User does not have this role." });

        await _userManager.RemoveFromRoleAsync(user, request.Role);
        return Ok(new { message = $"Role '{request.Role}' removed from {user.Email}." });
    }

    // GET api/admin/users/{userId}/roles
    [HttpGet("users/{userId}/roles")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }
}

public record RoleRequest(string Role);
