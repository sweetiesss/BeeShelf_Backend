using BeeStore_Repository.DTO.InventoryDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Inventory
{
    public class InventoryCreateDTOValidator : AbstractValidator<InventoryCreateDTO>
    {
        public InventoryCreateDTOValidator()
        {
            // Rule for Name: required and max length 25
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength);

            // Rule for MaxWeight: optional but must be non-negative if provided
            RuleFor(x => x.MaxWeight)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(0).When(x => x.MaxWeight.HasValue)
                .WithMessage(ValidationMessage.MaxWeightNonNegative);


            RuleFor(x => x.Price)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue)
                .WithMessage(ValidationMessage.WeightNonNegative);

            // Rule for WarehouseId: required
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage(ValidationMessage.WarehouseIdRequired);
        }
    }

}
