using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
    Guid? CategoryId,
    string? Search,
    string? Brand,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    string? SortBy,
    bool SortDesc,
    int Page,
    int PageSize,
    bool IncludePrice) : IRequest<PagedResult<ProductResponse>>;
