using BeeStore_Repository.DTO.UserDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.User
{
    public class UserSignUpRequestDTOValidator : AbstractValidator<UserSignUpRequestDTO>
    {
        public UserSignUpRequestDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmailRequired)
                .EmailAddress()
                .WithMessage(ValidationMessage.EmailInvalid)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.EmailMaxLength);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(ValidationMessage.FirstNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.FirstNameMaxLength);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(ValidationMessage.LastNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.LastNameMaxLength);

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage(ValidationMessage.PhoneRequired)
                .MaximumLength(11)
                .WithMessage(ValidationMessage.PhoneMaxLength);

            RuleFor(x => x.CitizenIdentificationNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.CitizenIdRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.CitizenIdMaxLength);

            RuleFor(x => x.TaxIdentificationNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.TaxIdRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.TaxIdMaxLength);

            RuleFor(x => x.BusinessName)
                .NotEmpty()
                .WithMessage(ValidationMessage.BusinessNameRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.BusinessNameMaxLength);

            RuleFor(x => x.BankName)
                .NotEmpty()
                .WithMessage(ValidationMessage.BankNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.BankNameMaxLength);

            RuleFor(x => x.BankAccountNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.BankAccountNumberRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.BankAccountNumberMaxLength);
            RuleFor(x => x.ProvinceId)
                .NotEmpty();
            RuleFor(x => x.CategoryId)
                .NotEmpty();
            RuleFor(x => x.OcopCategoryId)
                .NotEmpty();

        }
    }

}
