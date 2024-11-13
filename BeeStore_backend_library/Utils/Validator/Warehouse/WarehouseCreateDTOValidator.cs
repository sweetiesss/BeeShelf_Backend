using BeeStore_Repository.DTO.WarehouseDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Warehouse
{
    public class WarehouseCreateDTOValidator : AbstractValidator<WarehouseCreateDTO>
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
                .MaximumLength(50)
                .WithMessage(ValidationMessage.LocationMaxLength);

            RuleFor(x => x.CreateDate)
                .NotNull()
                .WithMessage(ValidationMessage.CreateDateRequired);
        }
    }
}
