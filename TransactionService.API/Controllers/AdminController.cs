using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Application.ActionFilter;
using TransactionService.Application.Commands;
using TransactionService.Application.DTO;

namespace TransactionService.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(policy: "RequireAdminRole")]
[ServiceFilter(typeof(TenantActionFilter))]
public class AdminController:ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("RollBack User transactions")]
    public async Task<IActionResult> RollBackUserTransactions([FromBody] RollbackDto rollbackDto)
    {
        await _mediator.Send(new RollbackCommand(rollbackDto));
        return Ok();
    }
    
    [HttpPost("RollBack All transactions")]
    public async Task<IActionResult> RollBackAllTransactions([FromBody] DateOnly rollbackDate)
    {
        RollbackDto rollbackDto = new RollbackDto
        {
            RollbackDate = rollbackDate,
            Username = null
        };
        await _mediator.Send(new RollbackCommand(rollbackDto));
        return Ok();
    }
    
}