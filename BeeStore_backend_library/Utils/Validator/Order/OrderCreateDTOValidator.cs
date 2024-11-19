using BeeStore_Repository.DTO.OrderDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Order
{
    public class OrderCreateDTOValidator : AbstractValidator<OrderCreateDTO>
    {
        public OrderCreateDTOValidator()
        {
            // Rule for OcopPartnerId: required
            RuleFor(x => x.OcopPartnerId)
                .NotNull()
                .WithMessage(ValidationMessage.OcopPartnerIdRequired);

            // Rule for ReceiverPhone: required and max length 11
            RuleFor(x => x.ReceiverPhone)
                .NotEmpty()
                .WithMessage(ValidationMessage.ReceiverPhoneRequired)
                .MaximumLength(11)
                .WithMessage(ValidationMessage.ReceiverPhoneMaxLength);

            // Rule for ReceiverAddress: required and max length 50
            RuleFor(x => x.ReceiverAddress)
                .NotEmpty()
                .WithMessage(ValidationMessage.ReceiverPhoneRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.ReceiverAddressMaxLength);

            //// Rule for OrderDetails: required and must contain at least one item
            //RuleFor(x => x.OrderDetails)
            //    .NotEmpty()
            //    .WithMessage(ValidationMessage.OrderDetailsNotEmpty);

            //// Nested validation for OrderDetailCreateDTO
            //RuleForEach(x => x.OrderDetails).SetValidator(new OrderDetailCreateDTOValidator());

            RuleFor(x => x.Products)
                .NotEmpty()
                .WithMessage(ValidationMessage.ProductDetailsNotEmpty);

            RuleForEach(x => x.Products).SetValidator(new ProductDetailDTOValidator());
        }
    }

    public class ProductDetailDTOValidator : AbstractValidator<ProductDetailDTO>
    {
        public ProductDetailDTOValidator()
        {
            // Rule for LotId: required
            RuleFor(x => x.ProductId)
                .NotNull()
                .WithMessage(ValidationMessage.ProductIdRequired);

            // Rule for ProductAmount: required and non-negative
            RuleFor(x => x.ProductAmount)
                .NotNull()
                .WithMessage(ValidationMessage.ProductAmountNonNegative)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessage.ProductAmountNonNegative);
        }
    }

}
