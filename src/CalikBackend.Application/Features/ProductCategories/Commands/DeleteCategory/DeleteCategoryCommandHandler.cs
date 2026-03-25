using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IProductCategoryRepository _repo;

    public DeleteCategoryCommandHandler(IProductCategoryRepository repo) => _repo = repo;

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _repo.GetByIdWithProductsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Category not found.");

        if (category.Products.Count > 0)
            throw new ConflictException("Cannot delete category with existing products.");

        _repo.Remove(category);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
