using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    string? Brand,
    string? Model,
    string? ImageUrl,
    decimal Price,
    int Stock,
    string Unit,
    Guid CategoryId) : IRequest<ProductResponse>;
