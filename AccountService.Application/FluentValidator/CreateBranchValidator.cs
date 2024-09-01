using BankingSystem.SharedLibrary.DTO;
using FluentValidation;

namespace AccountService.Application.FluentValidator;

public class CreateBranchValidator:AbstractValidator<CreateBranchDto>
{
    public CreateBranchValidator()
    {

        RuleFor(branch => branch.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .Must(EndWithBranch).WithMessage("Branch name must end with 'branch'.");
        
        RuleFor(branch => branch.Address)
            .NotEmpty().WithMessage("Address is required.")
            .Length(5, 250).WithMessage("Address must be between 5 and 250 characters.");
        
    }
    
    private bool EndWithBranch(string? branchName)
    {
        return branchName != null && branchName.EndsWith("branch");
    }
}