using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repo;

    public DeleteProductCommandHandler(IProductRepository repo) => _repo = repo;

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, includeCategory: false, cancellationToken)
            ?? throw new NotFoundException("Product not found.");

        _repo.Remove(product);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
