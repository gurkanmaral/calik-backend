using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IProductRepository _repo;

    public UpdateProductCommandHandler(IProductRepository repo) => _repo = repo;

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, includeCategory: true, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        if (!await _repo.CategoryExistsAsync(request.CategoryId, cancellationToken))
            throw new BadRequestException("Category not found.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Brand = request.Brand;
        product.Model = request.Model;
        product.ImageUrl = request.ImageUrl;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Unit = request.Unit;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(cancellationToken);

        // Reload category nav prop if changed
        var saved = await _repo.GetByIdAsync(product.Id, includeCategory: true, cancellationToken) ?? product;

        return new ProductResponse
        {
            Id = saved.Id,
            Name = saved.Name,
            Description = saved.Description,
            Brand = saved.Brand,
            Model = saved.Model,
            ImageUrl = saved.ImageUrl,
            Price = saved.Price,
            Stock = saved.Stock,
            Unit = saved.Unit,
            IsActive = saved.IsActive,
            CategoryId = saved.CategoryId,
            CategoryName = saved.Category?.Name ?? string.Empty,
            CreatedAt = saved.CreatedAt,
            UpdatedAt = saved.UpdatedAt
        };
    }
}
