using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.UpdateStock;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, ProductResponse>
{
    private readonly IProductRepository _repo;

    public UpdateStockCommandHandler(IProductRepository repo) => _repo = repo;

    public async Task<ProductResponse> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, includeCategory: true, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        product.Stock = request.Stock;
        product.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(cancellationToken);

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            Model = product.Model,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Stock = product.Stock,
            Unit = product.Unit,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
