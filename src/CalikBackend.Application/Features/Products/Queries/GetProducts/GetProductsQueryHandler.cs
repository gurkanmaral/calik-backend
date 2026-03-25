using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IProductRepository _repo;

    public GetProductsQueryHandler(IProductRepository repo) => _repo = repo;

    public Task<PagedResult<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        => _repo.GetPagedAsync(
            request.CategoryId, request.Search, request.Brand,
            request.MinPrice, request.MaxPrice, request.InStock,
            request.SortBy, request.SortDesc, request.Page, request.PageSize,
            request.IncludePrice, cancellationToken);
}
