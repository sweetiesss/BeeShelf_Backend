using BeeStore_Repository.DTO.WarehouseShipperDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.WarehouseShipper
{
    public class WarehouseShipperCreateDTOValidator : AbstractValidator<WarehouseShipperCreateDTO>
    {
        public WarehouseShipperCreateDTOValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmployeeIdRequired);

            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage(ValidationMessage.WarehouseIdRequired);

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage(ValidationMessage.StatusRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.StatusMaxLength);
        }
    }
}
