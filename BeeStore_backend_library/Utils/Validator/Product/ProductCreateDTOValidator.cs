using BeeStore_Repository.DTO.ProductDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator.Product
{
    public class ProductCreateDTOValidator : AbstractValidator<ProductCreateDTO>
    {
        public ProductCreateDTOValidator()
        {
            RuleFor(x => x.OcopPartnerId)
                .NotNull()
                .WithMessage(ValidationMessage.OcopPartnerIdRequired);

            RuleFor(x => x.Barcode)
                .NotEmpty()
                .WithMessage(ValidationMessage.BarcodeRequired)
                .MaximumLength(255)
                .WithMessage(ValidationMessage.BarcodeMaxLength);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.NameMaxLength);

            RuleFor(x => x.Price)
                .NotNull()
                .WithMessage(ValidationMessage.PriceRequired);

            RuleFor(x => x.Weight)
                .NotNull()
                .WithMessage(ValidationMessage.WeightRequired);

            RuleFor(x => x.ProductCategoryId)
                .NotNull()
                .WithMessage(ValidationMessage.ProductCategoryIdRequired);

            //RuleFor(x => x.PictureLink)
            //    .NotEmpty()
            //    .WithMessage(ValidationMessage.PictureLinkRequired);

            RuleFor(x => x.Origin)
                .NotEmpty()
                .WithMessage(ValidationMessage.OriginRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.OriginMaxLength);
        }
    }

}
