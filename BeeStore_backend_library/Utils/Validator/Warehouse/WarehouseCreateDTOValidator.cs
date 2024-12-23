using BeeStore_Repository.DTO.WarehouseDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Warehouse
{
    public class WarehouseCreateDTOValidator : AbstractValidator<StoreCreateDTO>
    {
        public WarehouseCreateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength);

            RuleFor(x => x.Capacity)
                .NotNull()
                .WithMessage(ValidationMessage.CapacityRequired);

            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage(ValidationMessage.LocationRequired)
                .MaximumLength(100)
                .WithMessage(ValidationMessage.LocationMaxLength);
        }
    }
}
