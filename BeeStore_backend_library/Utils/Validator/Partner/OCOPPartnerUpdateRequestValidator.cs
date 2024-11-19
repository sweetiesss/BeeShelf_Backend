using BeeStore_Repository.DTO.PartnerDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Partner
{
    public class OCOPPartnerUpdateRequestValidator : AbstractValidator<OCOPPartnerUpdateRequest>
    {
        public OCOPPartnerUpdateRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmailRequired)
                .EmailAddress()
                .WithMessage(ValidationMessage.EmailInvalid)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.EmailMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(ValidationMessage.ConfirmPasswordRequired)
                .When(x => !string.IsNullOrEmpty(x.ConfirmPassword));

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(ValidationMessage.FirstNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(ValidationMessage.LastNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage(ValidationMessage.PhoneRequired)
                .MaximumLength(11)
                .WithMessage(ValidationMessage.PhoneMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.CitizenIdentificationNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.CitizenIdRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.CitizenIdMaxLength)
                .When(x => !string.IsNullOrEmpty(x.CitizenIdentificationNumber));

            RuleFor(x => x.TaxIdentificationNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.TaxIdRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.TaxIdMaxLength)
                .When(x => !string.IsNullOrEmpty(x.TaxIdentificationNumber));

            RuleFor(x => x.BusinessName)
                .NotEmpty()
                .WithMessage(ValidationMessage.BusinessNameRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.BusinessNameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.BusinessName));

            RuleFor(x => x.BankName)
                .NotEmpty()
                .WithMessage(ValidationMessage.BankNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.BankNameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.BankName));

            RuleFor(x => x.BankAccountNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.BankAccountNumberRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.BankAccountNumberMaxLength)
                .When(x => !string.IsNullOrEmpty(x.BankAccountNumber));

            RuleFor(x => x.ProvinceId)
                .NotNull()
                .WithMessage(ValidationMessage.ProvinceIdRequired)
                .When(x => x.ProvinceId.HasValue);

            RuleFor(x => x.CategoryId)
                .NotNull()
                .WithMessage(ValidationMessage.CategoryIdRequired)
                .When(x => x.CategoryId.HasValue);

            RuleFor(x => x.OcopCategoryId)
                .NotNull()
                .WithMessage(ValidationMessage.OcopCategoryIdRequired)
                .When(x => x.OcopCategoryId.HasValue);
        }
    }

}
