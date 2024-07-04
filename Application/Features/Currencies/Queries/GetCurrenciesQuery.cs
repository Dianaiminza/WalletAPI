using Application.Boundary.Responses;
using Domain.Entities;
using Infrastructure.Repository.Abstractions;
using Infrastructure.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Application.Features.Currencies.Queries;

public class GetCurrenciesQuery : IRequest<Result<List<CurrencyResponse>>>
{
}

internal sealed class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, Result<List<CurrencyResponse>>>
{
  private readonly IRepositoryUnit _repository;

  public GetCurrenciesQueryHandler(IRepositoryUnit repository)
  {
    _repository = repository;
  }

  public async Task<Result<List<CurrencyResponse>>> Handle(
      GetCurrenciesQuery request,
      CancellationToken cancellationToken)
  {
    var currencies = await _repository
      .Entity<Currency>()
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    var response = currencies
      .Select(c => new CurrencyResponse
      {
        Id = c.Id,
        Name = c.Description,
        Code = c.Code,
      }).ToList();

    return await Result<List<CurrencyResponse>>.SuccessAsync(response, "Currencies Fetched Successfully");
  }
}
