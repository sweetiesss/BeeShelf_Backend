using BeeStore_Repository.DTO.PaymentDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Payment
{
    public class ConfirmPaymentDTOValidator : AbstractValidator<ConfirmPaymentDTO>
    {
        public ConfirmPaymentDTOValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(ValidationMessage.CodeRequired);

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(ValidationMessage.IdRequired);

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage(ValidationMessage.StatusRequired);

            RuleFor(x => x.OrderCode)
                .NotEmpty()
                .WithMessage(ValidationMessage.OrderCodeRequired);
        }
    }

}
