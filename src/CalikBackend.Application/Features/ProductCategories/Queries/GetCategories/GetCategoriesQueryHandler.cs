using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryResponse>>
{
    private readonly IProductCategoryRepository _repo;

    public GetCategoriesQueryHandler(IProductCategoryRepository repo) => _repo = repo;

    public Task<PagedResult<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        => _repo.GetPagedAsync(request.Search, request.SortDesc, request.Page, request.PageSize, cancellationToken);
}
