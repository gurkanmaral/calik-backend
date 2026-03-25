using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly IProductCategoryRepository _repo;

    public UpdateCategoryCommandHandler(IProductCategoryRepository repo) => _repo = repo;

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");

        category.Name = request.Name;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;

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
