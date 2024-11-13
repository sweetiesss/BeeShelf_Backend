using BeeStore_Repository.DTO.PaymentDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
