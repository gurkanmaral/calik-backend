using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly IProductCategoryRepository _repo;

    public CreateCategoryCommandHandler(IProductCategoryRepository repo) => _repo = repo;

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new ProductCategory
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };

        await _repo.AddAsync(category, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

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
