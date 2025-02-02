﻿using BeeStore_Repository.DTO.PackageDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.Lot
{
    public class LotCreateDTOValidator : AbstractValidator<LotCreateDTO>
    {
        public LotCreateDTOValidator()
        {
            // Rule for LotNumber: required and max length 25
            RuleFor(x => x.LotNumber)
                .NotEmpty()
                .WithMessage(ValidationMessage.LotNumberRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.LotNumberMaxLength);

            // Rule for Name: required and max length 50
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.NameMaxLength50);

            // Rule for Amount: required and non-negative
            RuleFor(x => x.LotAmount)
                .NotNull()
                .WithMessage(ValidationMessage.AmountNonNegative)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessage.AmountNonNegative);

            // Rule for ProductId: required
            RuleFor(x => x.ProductId)
                .NotNull()
                .WithMessage(ValidationMessage.ProductIdRqeuired);

            // Rule for ProductAmount: required and non-negative
            RuleFor(x => x.ProductPerLot)
                .NotNull()
                .WithMessage(ValidationMessage.ProductAmountNonNegative)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationMessage.ProductAmountNonNegative);
        }
    }


}
