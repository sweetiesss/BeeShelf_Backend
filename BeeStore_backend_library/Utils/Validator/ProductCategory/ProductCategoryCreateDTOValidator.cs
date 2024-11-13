using BeeStore_Repository.DTO.ProductCategoryDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator.ProductCategory
{
    public class ProductCategoryCreateDTOValidator : AbstractValidator<ProductCategoryCreateDTO>
    {
        public ProductCategoryCreateDTOValidator()
        {
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
                .WithMessage(ValidationMessage.ExpireInRequired);
        }
    }

}
