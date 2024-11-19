using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;

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
