using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _repo;

    public CreateProductCommandHandler(IProductRepository repo) => _repo = repo;

    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!await _repo.CategoryExistsAsync(request.CategoryId, cancellationToken))
            throw new BadRequestException("Category not found.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            Model = request.Model,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Stock = request.Stock,
            Unit = request.Unit,
            CategoryId = request.CategoryId
        };

        await _repo.AddAsync(product, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        // Reload with category
        var saved = await _repo.GetByIdAsync(product.Id, includeCategory: true, cancellationToken)
            ?? product;

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
