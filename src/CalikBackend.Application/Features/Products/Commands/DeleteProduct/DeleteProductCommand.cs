using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
