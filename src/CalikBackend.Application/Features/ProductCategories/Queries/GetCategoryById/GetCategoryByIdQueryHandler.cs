using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly IProductCategoryRepository _repo;

    public GetCategoryByIdQueryHandler(IProductCategoryRepository repo) => _repo = repo;

    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt
        };
    }
}
