using Application.Boundary.Requests;
using Application.Boundary.Responses;
using CoreApi.Infrastructure.Extensions;
using Domain.Entities;
using Infrastructure.Repository.Abstractions;
using Infrastructure.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Shared.Extensions;

namespace CoreApi.Application.Features.Currencies.Commands;

public class UpdateCurrencyCommand : IRequest<Result<CurrencyResponse>>
{
  public CurrencyUpdateRequest UpdateRequest { get; }

  public UpdateCurrencyCommand(CurrencyUpdateRequest updateRequest)
  {
    UpdateRequest = updateRequest;
  }
}

internal sealed class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Result<CurrencyResponse>>
{
  private readonly IRepositoryUnit _repository;

  public UpdateCurrencyCommandHandler(IRepositoryUnit repository)
  {
    _repository = repository;
  }

  public async Task<Result<CurrencyResponse>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
  {
    var currency = await _repository
      .Entity<Currency>()
      .Where(c => c.Id == request.UpdateRequest.Id)
      .TrackChanges(true)
      .FirstOrDefaultAsync(cancellationToken)
      .EnsureExistsAsync($"Currency with id: {request.UpdateRequest.Id} not found");

    currency.Code = request.UpdateRequest.Code;
    currency.Description = request.UpdateRequest.Name;

    _repository.DbContext().Currencies.Update(currency);
    await _repository.SaveAsync("Failed to update currency", cancellationToken);
    var response = new CurrencyResponse { Id = currency.Id, Name = currency.Description, Code = currency.Code };
    return await Result<CurrencyResponse>.SuccessAsync(response, "Currency updated successfully");
  }
}
