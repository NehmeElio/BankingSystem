using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.ActionFilter;
using TransactionService.Application.Commands;
using TransactionService.Application.DTO;

namespace TransactionService.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(policy: "RequireEmployeeRole")]
[ServiceFilter(typeof(TenantActionFilter))]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Add Daily Recurrent Transaction")]
    public async Task<IActionResult> AddDailyRecurrentTransaction(
        [FromBody] RecurrentTransactionDto recurrentTransactionDto)
    {
        await _mediator.Send(new AddRecurrentTransactionCommand(recurrentTransactionDto, "Daily"));
        return Created();
    }

    [HttpPost("Add Weekly Recurrent Transaction")]
    public async Task<IActionResult> AddWeeklyRecurrentTransaction(
        [FromBody] RecurrentTransactionDto recurrentTransactionDto)
    {
        await _mediator.Send(new AddRecurrentTransactionCommand(recurrentTransactionDto, "Weekly"));
        return Created();
    }

    [HttpPost("Add Monthly Recurrent Transaction")]
    public async Task<IActionResult> AddMonthlyRecurrentTransaction([FromBody] RecurrentTransactionDto recurrentTransactionDto)
    {
        await _mediator.Send(new AddRecurrentTransactionCommand(recurrentTransactionDto, "Monthly"));
        return Created();
    }
}











































