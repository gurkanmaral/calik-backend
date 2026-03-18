using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using CalikBackend.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? district,
        [FromQuery] decimal? minBalance,
        [FromQuery] decimal? maxBalance,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _db.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.Name.Contains(search) ||
                (c.Email != null && c.Email.Contains(search)) ||
                (c.PhoneNumber != null && c.PhoneNumber.Contains(search)));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(c => c.City == city);

        if (!string.IsNullOrWhiteSpace(district))
            query = query.Where(c => c.District == district);

        if (minBalance.HasValue)
            query = query.Where(c => c.Balance >= minBalance.Value);

        if (maxBalance.HasValue)
            query = query.Where(c => c.Balance <= maxBalance.Value);

        query = sortBy?.ToLower() switch
        {
            "balance"   => sortDesc ? query.OrderByDescending(c => c.Balance)   : query.OrderBy(c => c.Balance),
            "createdat" => sortDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            _           => sortDesc ? query.OrderByDescending(c => c.Name)      : query.OrderBy(c => c.Name),
        };

        var result = await query
            .Select(c => MapToResponse(c))
            .ToPagedResultAsync(page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
            return NotFound(new { message = "Customer not found." });

        return Ok(MapToResponse(customer));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            CountryCode = request.CountryCode,
            Email = request.Email,
            Address = request.Address,
            City = request.City,
            District = request.District,
            Balance = request.Balance
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, MapToResponse(customer));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
            return NotFound(new { message = "Customer not found." });

        customer.Name = request.Name;
        customer.PhoneNumber = request.PhoneNumber;
        customer.CountryCode = request.CountryCode;
        customer.Email = request.Email;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.District = request.District;
        customer.Balance = request.Balance;
        customer.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(MapToResponse(customer));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
            return NotFound(new { message = "Customer not found." });

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/balance")]
    public async Task<IActionResult> AdjustBalance(Guid id, [FromBody] AdjustBalanceRequest request)
    {
        var customer = await _db.Customers.FindAsync(id);

        if (customer == null)
            return NotFound(new { message = "Customer not found." });

        customer.Balance += request.Amount;
        customer.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(MapToResponse(customer));
    }

    private static CustomerResponse MapToResponse(Customer c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        PhoneNumber = c.PhoneNumber,
        CountryCode = c.CountryCode,
        Email = c.Email,
        Address = c.Address,
        City = c.City,
        District = c.District,
        Balance = c.Balance,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
