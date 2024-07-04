using Application.Boundary.Requests;
using Domain.Entities;
using Infrastructure.Repository.Abstractions;
using Infrastructure.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Shared.Extensions;

namespace CoreApi.Application.Features.Currencies.Commands;
public class DeleteCurrencyCommand : IRequest<Result<string>>
{
  public DeleteCurrencyCommand(CurrencyDeleteRequest deleteRequest)
  {
    DeleteRequest = deleteRequest;
  }

  public CurrencyDeleteRequest DeleteRequest { get; }

  internal sealed class DeleteCurrencyCommandCommandHandler : IRequestHandler<DeleteCurrencyCommand, Result<string>>
  {
    private readonly IRepositoryUnit _repositoryUnit;

    public DeleteCurrencyCommandCommandHandler(IRepositoryUnit repositoryUnit)
    {
      _repositoryUnit = repositoryUnit;
    }

    public async Task<Result<string>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
      var currency = await _repositoryUnit.Entity<Currency>()
        .Where(tip => tip.Id == request.DeleteRequest.Id)
        .SingleOrDefaultAsync(cancellationToken)
        .EnsureExistsAsync($"Currency with ID {request.DeleteRequest.Id} not Found");
      currency.IsDeleted = true;
      _repositoryUnit.DbContext().Remove(currency);

      await _repositoryUnit.SaveAsync("Failed to delete currency", cancellationToken);

      return await Result<string>.SuccessAsync("Currency successfully deleted");
    }
  }
}
