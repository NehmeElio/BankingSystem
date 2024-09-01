
using AccountService.Persistence.Models;
using FluentValidation;

namespace AccountService.Application.FluentValidator;

public class RoleValidator:AbstractValidator<Role>
{
    public RoleValidator()
    {
        RuleFor(role => role.RoleName)
            .NotNull().NotEmpty().WithMessage("Role Name cannot be null or empty");
    }
    
}