using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Queries.GetCategories;

public record GetCategoriesQuery(
    string? Search,
    bool SortDesc,
    int Page,
    int PageSize) : IRequest<PagedResult<CategoryResponse>>;
