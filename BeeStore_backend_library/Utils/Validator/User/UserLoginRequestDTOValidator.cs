using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator.User
{
    public class UserLoginRequestDTOValidator : AbstractValidator<UserLoginRequestDTO>
    {
        public UserLoginRequestDTOValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmailRequired)
                .EmailAddress()
                .WithMessage(ValidationMessage.EmailInvalid)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.EmailMaxLength);

            RuleFor(x => x.password)
                .NotEmpty()
                .WithMessage(ValidationMessage.PasswordRequired)
                .MinimumLength(8)
                .WithMessage(ValidationMessage.PasswordMinLength)
                .MaximumLength(100)
                .WithMessage(ValidationMessage.PasswordMaxLength);
        }
    }

}
