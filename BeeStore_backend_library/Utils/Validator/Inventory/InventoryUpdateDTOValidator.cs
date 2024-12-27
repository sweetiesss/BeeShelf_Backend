using BeeStore_Repository.DTO.InventoryDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Inventory
{
    public class InventoryUpdateDTOValidator : AbstractValidator<RoomUpdateDTO>
    {
        public InventoryUpdateDTOValidator()
        {
            RuleFor(x => x.RoomCode)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.RoomCode));

            RuleFor(x => x.MaxWeight)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessage.MaxWeightNonNegative)
                .When(x => x.MaxWeight.HasValue);

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessage.WeightNonNegative)
                .When(x => x.Weight.HasValue);
        }
    }
}
