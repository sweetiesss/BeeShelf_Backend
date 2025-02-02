﻿using BeeStore_Repository.DTO.ProductCategoryDTOs;
using FluentValidation;

namespace BeeStore_Repository.Utils.Validator.ProductCategory
{
    public class ProductCategoryCreateDTOValidator : AbstractValidator<ProductCategoryCreateDTO>
    {
        public ProductCategoryCreateDTOValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .NotNull()
                .NotEqual(0)
                .WithMessage(ValidationMessage.CategoryIdRequired);

            RuleFor(x => x.TypeName)
                .NotEmpty()
                .WithMessage(ValidationMessage.TypeNameRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.TypeNameMaxLength);

            RuleFor(x => x.TypeDescription)
                .NotEmpty()
                .WithMessage(ValidationMessage.TypeDescriptionRequired)
                .MaximumLength(100)
                .WithMessage(ValidationMessage.TypeDescriptionMaxLength);

            RuleFor(x => x.ExpireIn)
                .NotNull()
                .NotEmpty()
                .NotEqual(0)
                .WithMessage(ValidationMessage.ExpireInRequired);
        }
    }

}
