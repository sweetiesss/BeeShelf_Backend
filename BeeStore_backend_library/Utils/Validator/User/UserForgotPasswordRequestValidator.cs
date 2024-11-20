using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
