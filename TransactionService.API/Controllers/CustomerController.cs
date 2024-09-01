using AutoMapper;
using BankingSystem.SharedLibrary.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using TransactionService.Application.ActionFilter;
using TransactionService.Application.Commands;
using TransactionService.Application.ViewModel;


namespace TransactionService.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(policy:"RequireCustomerRole")]
[ServiceFilter(typeof(TenantActionFilter))]
public class CustomerController:ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CustomerController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("Deposit")]
    public async Task<IActionResult> Deposit([FromBody] decimal amount)
    {
        var user = HttpContext.User;
        await _mediator.Send(new DepositCommand(amount,user));
        
        return Created();
    }
    
    [HttpPost("Withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] decimal amount)
    {
        var user = HttpContext.User;
        await _mediator.Send(new WithDrawCommand(amount,user));
        
        return Created();
    }

    [HttpPost("Get Account Information")]
    public async Task<IActionResult> GetAccountInformation()
    {
        var user = HttpContext.User;
        var username=user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        if (username == null) throw new NotFoundException("Username missing from claims");
        
        var result=await _mediator.Send(new GetAccountInformationCommand(username));
        

        return Ok(result);

    }
    
    [HttpPost("Transactions")]
    [EnableQuery]
    public async Task<IActionResult> GetTransactions()
    {
        var user = HttpContext.User;
        var username=user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        if (username == null) throw new NotFoundException("Username missing from claims");
        
        var transactions=await _mediator.Send(new GetTransactionsCommand(username));

        var result=transactions.Select(t => _mapper.Map<TransactionViewModel>(t)).ToList();

        return Ok(result);

    }

    
}