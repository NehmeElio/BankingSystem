using AccountService.Domain.Models;
using BankingSystem.SharedLibrary.Interfaces;
using Keycloak.Net;
using Keycloak.Net.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionService.Persistence.Context;
using TransactionService.Persistence.Interfaces;
using Account = TransactionService.Domain.Models.Account;

namespace TransactionService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TestingController:ControllerBase
{

    private readonly ITokenService _tokenService;
    

    public TestingController(ITokenService tokenService)
    {

        _tokenService = tokenService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login( string user,  string pass)
    {
        var token = await _tokenService.GetTokenAsync(user, pass);
        return Ok(token);
    }


}