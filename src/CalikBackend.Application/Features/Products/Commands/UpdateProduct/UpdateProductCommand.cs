using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Brand,
    string? Model,
    string? ImageUrl,
    decimal Price,
    int Stock,
    string Unit,
    Guid CategoryId,
    bool IsActive) : IRequest<ProductResponse>;
