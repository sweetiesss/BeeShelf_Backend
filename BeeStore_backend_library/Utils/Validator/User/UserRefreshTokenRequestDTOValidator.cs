using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator.User
{
    public class UserRefreshTokenRequestDTOValidator : AbstractValidator<UserRefreshTokenRequestDTO>
    {
        public UserRefreshTokenRequestDTOValidator()
        {
            RuleFor(x => x.Jwt)
                .NotEmpty()
                .WithMessage(ValidationMessage.JwtRequired);
        }
    }

}
