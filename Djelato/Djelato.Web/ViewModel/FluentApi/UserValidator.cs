using Djelato.Common.Entity;
using Djelato.Common.Shared;
using FluentValidation;

namespace Djelato.Web.ViewModel.FluentApi
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(u => u.Avatar).NotNull().WithMessage("Please choose profile avatar");

            RuleFor(u => u.Name).NotEmpty().WithMessage("Name field cant be empty")
                .NotNull().WithMessage("Name field should be fill in")
                .Matches(RegexExpressions.NameRgx).WithMessage("Field should use latin alphabet");

            RuleFor(u => u.Email).NotEmpty().WithMessage("Email field cant be empty")
                .NotNull().WithMessage("Email field should be fill in")
                .Matches(RegexExpressions.EmailRgx).WithMessage("Email has incorrect format");

            RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Phone number field cant be empty")
                .NotNull().WithMessage("Phone number field should be fill in")
                .Matches(RegexExpressions.PhoneRgx).WithMessage("Phone number has incorrect format");

            RuleFor(u => u.Password).NotEmpty().WithMessage("Password field cant be empty")
                .NotNull().WithMessage("Password field should be fill in")
                .Matches(RegexExpressions.PasswordRgx).WithMessage("Should be minimum 8 elements, one uppercase letter, one number");

            RuleFor(u => u.PasswordConfirm).Equal(pas => pas.Password).WithMessage("Confirm field is mismatch");

            RuleFor(u => u.Role).IsInEnum<UserDTO, Role>().WithMessage("Role is incorrect");
        }
    }
}
