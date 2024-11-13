using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using FluentValidation;
using BeeStore_Repository.Utils;

namespace BeeStore_Repository.Utils.Validator.WarehouseStaff
{
    public class WarehouseStaffCreateDTOValidator : AbstractValidator<WarehouseStaffCreateDTO>
    {
        public WarehouseStaffCreateDTOValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmployeeIdRequired);

            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage(ValidationMessage.WarehouseIdRequired);
        }
    }
}
