using BeeStore_Repository.DTO.PaymentDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Payment
{
    public class PaymentRequestDTOValidator : AbstractValidator<PaymentRequestDTO>
    {
        public PaymentRequestDTOValidator()
        {
            RuleFor(x => x.BuyerEmail)
                .EmailAddress()
                .WithMessage(ValidationMessage.EmailInvalid)
                .NotEmpty()
                .WithMessage(ValidationMessage.BuyerEmailRequired);

            RuleFor(x => x.CancelUrl)
                .NotEmpty()
                .WithMessage(ValidationMessage.CancelUrlRequired);

            RuleFor(x => x.ReturnUrl)
                .NotEmpty()
                .WithMessage(ValidationMessage.ReturnUrlRequired);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(ValidationMessage.DescriptionRequired);
        }
    }

}
