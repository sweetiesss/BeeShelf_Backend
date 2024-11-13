using BeeStore_Repository.DTO.OrderDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Order
{
    public class OrderUpdateDTOValidator : AbstractValidator<OrderUpdateDTO>
    {
        public OrderUpdateDTOValidator()
        {
            RuleFor(x => x.ReceiverPhone)
                .MaximumLength(11)
                .WithMessage(ValidationMessage.ReceiverPhoneMaxLength)
                .When(x => !string.IsNullOrEmpty(x.ReceiverPhone));

            RuleFor(x => x.ReceiverAddress)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.ReceiverAddressMaxLength)
                .When(x => !string.IsNullOrEmpty(x.ReceiverAddress));

            RuleFor(x => x.OrderDetails)
                .NotEmpty()
                .WithMessage(ValidationMessage.OrderDetailsNotEmpty)
                .When(x => x.OrderDetails != null && x.OrderDetails.Any());

            RuleForEach(x => x.OrderDetails).SetValidator(new OrderDetailUpdateDTOValidator());
        }
    }

    public class OrderDetailUpdateDTOValidator : AbstractValidator<OrderDetailUpdateDTO>
    {
        public OrderDetailUpdateDTOValidator()
        {
            RuleFor(x => x.ProductAmount)
                .NotNull()
                .WithMessage(ValidationMessage.ProductAmountNonNegative)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessage.ProductAmountNonNegative)
                .When(x => x.ProductAmount.HasValue);
        }
    }
}
