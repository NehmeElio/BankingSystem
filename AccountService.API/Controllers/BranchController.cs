
using AccountService.Application.Commands;
using BankingSystem.SharedLibrary.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(policy: "RequireAdminRole")]
public class BranchController:ControllerBase
{
    
    private readonly IMediator _mediator;

    public BranchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("CreateBranch")]
    public async Task<IActionResult> CreateNewBranch([FromBody] CreateBranchDto newBranch)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateBranchCommand(newBranch);

        await _mediator.Send(command);
        return Accepted($"Branch creation initiated for {newBranch.BranchName}.");

    }

}