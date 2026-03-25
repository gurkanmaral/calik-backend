using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl) : IRequest<CategoryResponse>;
