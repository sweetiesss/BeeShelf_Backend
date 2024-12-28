using BeeStore_Repository.DTO.WarehouseStaffDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.WarehouseStaff
{
    public class WarehouseStaffCreateDTOValidator : AbstractValidator<StoreStaffCreateDTO>
    {
        public WarehouseStaffCreateDTOValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage(ValidationMessage.EmployeeIdRequired);

            RuleFor(x => x.StoreId)
                .NotEmpty()
                .WithMessage(ValidationMessage.WarehouseIdRequired);
        }
    }
}
