using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    string? ImageUrl) : IRequest<CategoryResponse>;
