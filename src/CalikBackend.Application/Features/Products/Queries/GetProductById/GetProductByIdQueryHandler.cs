using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _repo;

    public GetProductByIdQueryHandler(IProductRepository repo) => _repo = repo;

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, includeCategory: true, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            Model = product.Model,
            ImageUrl = product.ImageUrl,
            Price = request.IncludePrice ? product.Price : null,
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
