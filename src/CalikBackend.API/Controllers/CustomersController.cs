using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Features.Customers.Commands.AdjustBalance;
using CalikBackend.Application.Features.Customers.Commands.CreateCustomer;
using CalikBackend.Application.Features.Customers.Commands.DeleteCustomer;
using CalikBackend.Application.Features.Customers.Commands.UpdateCustomer;
using CalikBackend.Application.Features.Customers.Queries.GetCustomerById;
using CalikBackend.Application.Features.Customers.Queries.GetCustomers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin")]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender) => _sender = sender;

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
        => Ok(await _sender.Send(new GetCustomersQuery(search, city, district, minBalance, maxBalance, sortBy, sortDesc, page, pageSize)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _sender.Send(new GetCustomerByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var result = await _sender.Send(new CreateCustomerCommand(
            request.Name, request.PhoneNumber, request.CountryCode,
            request.Email, request.Address, request.City, request.District, request.Balance));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
        => Ok(await _sender.Send(new UpdateCustomerCommand(
            id, request.Name, request.PhoneNumber, request.CountryCode,
            request.Email, request.Address, request.City, request.District, request.Balance)));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteCustomerCommand(id));
        return NoContent();
    }

    [HttpPatch("{id}/balance")]
    public async Task<IActionResult> AdjustBalance(Guid id, [FromBody] AdjustBalanceRequest request)
        => Ok(await _sender.Send(new AdjustBalanceCommand(id, request.Amount)));
}
