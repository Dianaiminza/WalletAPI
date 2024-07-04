using Application.Boundary.Requests;
using Application.Boundary.Responses;
using Application.Features.Currencies.Commands;
using CoreApi.Application.Features.Currencies.Commands;
using CoreApi.Application.Features.Currencies.Queries;
using Infrastructure.Shared.Wrapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace WalletAPI.Controllers
{

    [Route("api/v{version:apiVersion}/wallet")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DailyTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DailyTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Currencies

        /// <summary>
        /// Get All Currencies
        /// </summary>
        /// <returns></returns>
        [HttpGet("currencies")]
        public async Task<ActionResult<Result<List<CurrencyResponse>>>> GetAllCurrenciesAsync()
        {
            var query = new GetCurrenciesQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        /// <summary>
        /// Create Currency
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("currencies/create")]
        public async Task<ActionResult<Result<CurrencyResponse>>> CreateCurrencyAsync(
          [FromBody] CurrencyCreationRequest request)
        {
            var command = new CreateCurrencyCommand(request);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Update Currency
        /// </summary>
        /// <param name="currencyId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("currencies/{currencyId}")]
        public async Task<ActionResult<Result<CurrencyResponse>>> UpdateCurrencyAsync([FromRoute] long currencyId,
          [FromBody] CurrencyUpdateRequest request)
        {
            request.Id = currencyId;
            var command = new UpdateCurrencyCommand(request);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Export currencies
        /// </summary>
        /// <returns></returns>
        [HttpGet("currencies/export")]
        public async Task<ActionResult<string>> ExportCurrencies()
        {
            return Ok(await _mediator.Send(new ExportCurrenciesQuery()));
        }

        /// <summary>
        /// Delete currency
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("currencies/delete")]
        public async Task<ActionResult<Result<string>>> DeleteCurrency([FromBody] CurrencyDeleteRequest request)
        {
            var response = await _mediator.Send(new DeleteCurrencyCommand(request));
            return Ok(response);
        }

        #endregion


    }
}
