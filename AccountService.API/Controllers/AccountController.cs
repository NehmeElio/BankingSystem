using AccountService.Application.ActionFilter;
using AccountService.Application.Commands;
using BankingSystem.SharedLibrary.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.API.Controllers;


[ApiController]
[Route("[controller]")]
public class AccountController(IMediator mediator) : ControllerBase
{

    [HttpPost("Create customer account")]
    [ServiceFilter(typeof(TenantActionFilter))]
    [Authorize(policy:"RequireEmployeeRole")]
    public async Task<IActionResult> CreateAccount(AddAccountDto addAccountDto)
    {
        await mediator.Send(new CreateAccountCommand(addAccountDto));
        return Ok();
    }
    
    [HttpPost("Create employee account")]
    [ServiceFilter(typeof(TenantActionFilter))]
    [Authorize("RequireAdminRole")]
    public async Task<IActionResult> CreateEmployeeAccount(AddUserDto addUserDto)
    {
        await mediator.Send(new CreateEmployeeCommand(addUserDto));
        return Ok();
    }
}