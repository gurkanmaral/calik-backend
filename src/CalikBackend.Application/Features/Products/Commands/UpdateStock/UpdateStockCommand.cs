using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.UpdateStock;

public record UpdateStockCommand(Guid Id, int Stock) : IRequest<ProductResponse>;
