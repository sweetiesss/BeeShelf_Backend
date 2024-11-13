using BeeStore_Repository.DTO.VehicleDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Vehicle
{
    public class VehicleCreateDTOValidator : AbstractValidator<VehicleCreateDTO>
    {
        public VehicleCreateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength);

            RuleFor(x => x.LicensePlate)
                .NotEmpty()
                .WithMessage(ValidationMessage.LicensePlateRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.LicensePlateMaxLength);

            RuleFor(x => x.Capacity)
                .NotNull()
                .WithMessage(ValidationMessage.CapacityRequired);

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage(ValidationMessage.TypeRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.TypeMaxLength);

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage(ValidationMessage.StatusRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.StatusMaxLength);
        }
    }
}
