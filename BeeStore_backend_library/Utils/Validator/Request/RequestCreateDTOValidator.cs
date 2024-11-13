using BeeStore_Repository.DTO.RequestDTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator.Request
{
    public class RequestCreateDTOValidator : AbstractValidator<RequestCreateDTO>
    {
        public RequestCreateDTOValidator()
        {
            RuleFor(x => x.OcopPartnerId)
                .NotNull()
                .WithMessage(ValidationMessage.OcopPartnerIdRequired);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.RequestNameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.RequestNameMaxLength);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(ValidationMessage.RequestDescriptionRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.RequestDescriptionMaxLength);

            RuleFor(x => x.SendToInventoryId)
                .NotNull()
                .WithMessage(ValidationMessage.SendToInventoryIdRequired);

            RuleFor(x => x.Lot)
                .NotNull()
                .WithMessage(ValidationMessage.LotRequired);

            RuleFor(x => x.RequestType)
                .NotEmpty()
                .WithMessage(ValidationMessage.RequestTypeRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.RequestTypeMaxLength);

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage(ValidationMessage.StatusRequired)
                .MaximumLength(10)
                .WithMessage(ValidationMessage.StatusMaxLength);

            RuleFor(x => x.CancellationReason)
                .NotEmpty()
                .WithMessage(ValidationMessage.CancellationReasonRequired)
                .MaximumLength(50)
                .WithMessage(ValidationMessage.CancellationReasonMaxLength);
        }
    }

}
