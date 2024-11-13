using BeeStore_Repository.DTO.VehicleDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Vehicle
{
    public class VehicleUpdateDTOValidator : AbstractValidator<VehicleUpdateDTO>
    {
        public VehicleUpdateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.LicensePlate)
                .NotEmpty()
                .WithMessage(ValidationMessage.LicensePlateRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.LicensePlateMaxLength)
                .When(x => !string.IsNullOrEmpty(x.LicensePlate));

            RuleFor(x => x.Capacity)
                .NotNull()
                .WithMessage(ValidationMessage.CapacityRequired)
                .When(x => x.Capacity.HasValue);

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage(ValidationMessage.TypeRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.TypeMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Type));

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage(ValidationMessage.StatusRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.StatusMaxLength)
                .When(x => !string.IsNullOrEmpty(x.Status));
        }
    }
}
