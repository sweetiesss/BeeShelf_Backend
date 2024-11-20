using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.User
{
    public class UserForgotPasswordRequestValidator : AbstractValidator<UserForgotPasswordRequest>
    {
        public UserForgotPasswordRequestValidator()
        {
            RuleFor(x => x.token)
                .NotEmpty();
            RuleFor(x => x.newPassword)
                 .NotEmpty()
                .WithMessage(ValidationMessage.PasswordRequired)
                .MaximumLength(100)
                .WithMessage(ValidationMessage.PasswordMaxLength);

        }
    }
}
