using Djelato.Common.Shared;
using FluentValidation;

namespace Djelato.Web.ViewModel.FluentApi
{
    public class AuthValidator : AbstractValidator<AuthDTO>
    {
        public AuthValidator()
        {
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email field cant be empty")
                .NotNull().WithMessage("Email field should be fill in")
                .Matches(RegexExpressions.EmailRgx).WithMessage("Email has incorrect format");

            RuleFor(u => u.Password).NotEmpty().WithMessage("Password field cant be empty")
                .NotNull().WithMessage("Password field should be fill in")
                .Matches(RegexExpressions.PasswordRgx).WithMessage("Should be minimum 8 elements, one uppercase letter, one number");
        }
    }
}
