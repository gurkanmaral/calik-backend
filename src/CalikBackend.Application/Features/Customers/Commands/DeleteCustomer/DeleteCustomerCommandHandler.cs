using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly ICustomerRepository _repo;

    public DeleteCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");

        _repo.Remove(customer);
        await _repo.SaveChangesAsync(cancellationToken);
    }
}
