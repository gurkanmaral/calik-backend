using CalikBackend.Application.DTOs.Products;
using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryResponse>;
