using BankingSystem.SharedLibrary.DTO;
using MediatR;

namespace AccountService.Application.Commands;

// Domain/Commands/CreateBranchSchemaCommand.cs
public record CreateBranchCommand(CreateBranchDto NewBranch, string SourceSchemaName = "beirutbranch") : IRequest<Unit>;
