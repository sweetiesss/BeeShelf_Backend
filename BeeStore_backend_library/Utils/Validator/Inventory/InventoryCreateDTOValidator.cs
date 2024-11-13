using BeeStore_Repository.DTO.InventoryDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .GreaterThanOrEqualTo(0).When(x => x.MaxWeight.HasValue)
                .WithMessage(ValidationMessage.MaxWeightNonNegative);

            // Rule for Weight: optional but must be non-negative if provided
            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0).When(x => x.Weight.HasValue)
                .WithMessage(ValidationMessage.WeightNonNegative);

            // Rule for WarehouseId: required
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage(ValidationMessage.WarehouseIdRequired);
        }
    }

}
