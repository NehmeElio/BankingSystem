using AccountService.Application.ActionFilter;
using AccountService.Application.Commands;
using AccountService.Persistence.Context;
using BankingSystem.SharedLibrary.DTO;
using BankingSystem.SharedLibrary.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AccountService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ITokenService tokenService, BankContext context) : ControllerBase
{
    
    private readonly BankContext _context = context;


    
    [HttpGet]
    public async Task<IActionResult> GetToken( string username, string password) 
    {
        var token = await tokenService.GetTokenAsync(username,password);
        //if(!token.IsNullOrEmpty()) context.Response.Headers.Add("Authorization", "Bearer " + token);
        return Ok(token);
    }

    
    [HttpGet("Get-Accounts")]
    [ServiceFilter(typeof(TenantActionFilter))]
    public async Task<IActionResult> GetAccounts()
    {
        return await _context.Accounts.AnyAsync() ? Ok(_context.Accounts) : NotFound();
        return Ok();
    }
}