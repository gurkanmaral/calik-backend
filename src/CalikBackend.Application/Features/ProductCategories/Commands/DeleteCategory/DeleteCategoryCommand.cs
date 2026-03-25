using MediatR;

namespace CalikBackend.Application.Features.ProductCategories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;
