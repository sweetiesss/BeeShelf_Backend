using BeeStore_Repository.DTO.PartnerDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Partner
{
    public class PartnerUpdateRequestValidator : AbstractValidator<PartnerUpdateRequest>
    {
        public PartnerUpdateRequestValidator()
        {
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

            RuleFor(x => x.CitizenIdentificationNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.CitizenIdRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.CitizenIdMaxLength)
                .When(x => !string.IsNullOrEmpty(x.CitizenIdentificationNumber));

            RuleFor(x => x.UserId)
                .NotNull()
                .WithMessage(ValidationMessage.UserIdRequired)
                .When(x => x.UserId.HasValue);
        }
    }
}
