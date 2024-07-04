using Application.Boundary.Requests;
using Application.Boundary.Responses;
using Domain.Entities;
using Infrastructure.Repository.Abstractions;
using Infrastructure.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Features.Currencies.Commands;

public class CreateCurrencyCommand : IRequest<Result<CurrencyResponse>>
{
    public CurrencyCreationRequest CurrencyCreationRequest { get; }

    public CreateCurrencyCommand(CurrencyCreationRequest currencyCreationRequest)
    {
        CurrencyCreationRequest = currencyCreationRequest;
    }
}

internal sealed class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Result<CurrencyResponse>>
{
    private readonly IRepositoryUnit _repository;

    public CreateCurrencyCommandHandler(IRepositoryUnit repository)
    {
        _repository = repository;
    }

    public async Task<Result<CurrencyResponse>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var newCurrency = new Currency
        {
            Code = request.CurrencyCreationRequest.Code,
            Description = request.CurrencyCreationRequest.Name
        };

        var matchingCurrency = await _repository
          .Entity<Currency>()
          .Where(c => c.Code == request.CurrencyCreationRequest.Code ||
                      c.Description == request.CurrencyCreationRequest.Name)
          .FirstOrDefaultAsync(cancellationToken);

        if (matchingCurrency == null)
        {
            await _repository.DbContext().Currencies.AddAsync(newCurrency, cancellationToken);
            await _repository.SaveAsync("Failed to create currency", cancellationToken);
        }
        else
        {
            return await Result<CurrencyResponse>.FailAsync("A similar currency has already been added. Please try again");
        }

        var response = new CurrencyResponse
        {
            Name = request.CurrencyCreationRequest.Name,
            Code = request.CurrencyCreationRequest.Code
        };
        return await Result<CurrencyResponse>.SuccessAsync(response, "Currency created successfully");
    }
}
