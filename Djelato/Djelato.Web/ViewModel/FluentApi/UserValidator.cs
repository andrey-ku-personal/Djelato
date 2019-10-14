using Djelato.Common.Entity;
using Djelato.Common.Shared;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Djelato.Web.ViewModel.FluentApi
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(u => u.Name).NotEmpty().WithMessage("Name field cant be empty");
            RuleFor(u => u.Name).NotNull().WithMessage("Name field should be fill in");
            RuleFor(u => u.Name).Matches(RegexExpressions.NameRgx).WithMessage("Field should use latin alphabet");

            RuleFor(u => u.Email).NotEmpty().WithMessage("Email field cant be empty");
            RuleFor(u => u.Email).NotNull().WithMessage("Email field should be fill in");
            RuleFor(u => u.Email).Matches(RegexExpressions.EmailRgx).WithMessage("Email has incorrect format");

            RuleFor(u => u.Password).NotEmpty().WithMessage("Password field cant be empty");
            RuleFor(u => u.Password).NotNull().WithMessage("Password field should be fill in");
            RuleFor(u => u.Password).Matches(RegexExpressions.PasswordRgx).WithMessage("Should be minimum 8 elements, one uppercase letter, one number");

            RuleFor(u => u.ConfirmPassword).Equal(pas => pas.Password).WithMessage("Confirm field is mismatch");

            RuleFor(u => u.Role).IsInEnum<UserDTO, Role>().WithMessage("Role is incorrect");
        }
    }
}
