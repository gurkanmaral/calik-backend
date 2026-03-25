using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Application.Features.Invoices.Commands.CreateInvoice;
using CalikBackend.Application.Features.Invoices.Commands.DeleteInvoice;
using CalikBackend.Application.Features.Invoices.Commands.UpdateInvoice;
using CalikBackend.Application.Features.Invoices.Commands.UpdateInvoiceStatus;
using CalikBackend.Application.Features.Invoices.Queries.GetInvoiceById;
using CalikBackend.Application.Features.Invoices.Queries.GetInvoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize(Roles = "Admin")]
public class InvoicesController : ControllerBase
{
    private readonly ISender _sender;

    public InvoicesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDesc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await _sender.Send(new GetInvoicesQuery(
            search, status, dateFrom, dateTo, minAmount, maxAmount,
            sortBy, sortDesc, page, pageSize)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _sender.Send(new GetInvoiceByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
    {
        var result = await _sender.Send(new CreateInvoiceCommand(
            request.InvoiceDate, request.CustomerName, request.CustomerAddress,
            request.CustomerTaxNumber, request.CustomerEmail, request.CustomerPhone,
            request.Notes, request.Items));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceRequest request)
        => Ok(await _sender.Send(new UpdateInvoiceCommand(
            id, request.InvoiceDate, request.CustomerName, request.CustomerAddress,
            request.CustomerTaxNumber, request.CustomerEmail, request.CustomerPhone,
            request.Notes, request.Items)));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusRequest request)
        => Ok(await _sender.Send(new UpdateInvoiceStatusCommand(id, request.Status)));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteInvoiceCommand(id));
        return NoContent();
    }
}
