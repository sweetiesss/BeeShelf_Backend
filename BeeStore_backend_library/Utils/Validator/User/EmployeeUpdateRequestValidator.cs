using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.User
{
    public class EmployeeUpdateRequestValidator : AbstractValidator<EmployeeUpdateRequest>
    {
        public EmployeeUpdateRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmailRequired)
                .EmailAddress()
                .WithMessage(ValidationMessage.EmailInvalid)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.EmailMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Email));

            //RuleFor(x => x.ConfirmPassword)
            //    .NotEmpty()
            //    .WithMessage(ValidationMessage.ConfirmPasswordRequired)
            //    .Equal(x => x.ConfirmPassword)
            //    .WithMessage(ValidationMessage.PasswordMismatch)
            //    .When(x => !string.IsNullOrEmpty(x.ConfirmPassword));

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(ValidationMessage.FirstNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.FirstNameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(ValidationMessage.LastNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.LastNameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage(ValidationMessage.PhoneRequired)
                .MaximumLength(11)
                .WithMessage(ValidationMessage.PhoneMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }
    }
}
