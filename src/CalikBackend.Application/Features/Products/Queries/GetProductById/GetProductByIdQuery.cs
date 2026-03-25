using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id, bool IncludePrice) : IRequest<ProductResponse>;
