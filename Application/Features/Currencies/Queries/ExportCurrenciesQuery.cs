using Domain.Entities;
using Infrastructure.Repository.Abstractions;
using Infrastructure.Shared.Services.Abstractions;
using Infrastructure.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Application.Features.Currencies.Queries;

public class ExportCurrenciesQuery : IRequest<Result<string>>
{
    public ExportCurrenciesQuery()
    {

    }
    internal class ExportCurrenciesQueryHandler : IRequestHandler<ExportCurrenciesQuery, Result<string>>
    {
        private readonly IExcelService _excelService;

        private readonly IRepositoryUnit _repository;

        public ExportCurrenciesQueryHandler(IExcelService excelService, IRepositoryUnit repository)
        {
            _excelService = excelService;
            _repository = repository;
        }

        public async Task<Result<string>> Handle(ExportCurrenciesQuery request,
          CancellationToken cancellationToken)
        {
            {
                var currencies = await _repository.Entity<Currency>()
                  .ToListAsync(cancellationToken);


                var data = await _excelService.ExportAsync(currencies,
                  new Dictionary<string, Func<Currency, object>>
                  {
            { "Id", currencies => currencies.Id },
            { "Description", currencies => currencies.Description },
            { "Code", currencies => currencies.Code },
            { "CreatedOn", currencies => currencies.CreatedOn },
            { "LastModifiedOn", currencies => currencies.LastModifiedOn },
                  }, sheetName: "Currencies");

                return await Result<string>.SuccessAsync(data: data, "Success");
            }

        }
    }
}