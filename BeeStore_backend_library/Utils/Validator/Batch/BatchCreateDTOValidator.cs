using BeeStore_Repository.DTO.Batch;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Utils.Validator.BatchDTOs
{
    public class BatchCreateDTOValidator : AbstractValidator<BatchCreateDTO>
    {
        public BatchCreateDTOValidator()
        {
            // Rule for Name: must not be null, empty, or exceed 25 characters
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessage.NameRequired)
                .MaximumLength(25)
                .WithMessage(ValidationMessage.NameMaxLength);

            // Rule for Orders: must contain at least one item
            RuleFor(x => x.Orders)
                .NotNull()
                .WithMessage(ValidationMessage.OrdersNotNull)
                .Must(orders => orders != null && orders.Count > 0)
                .WithMessage(ValidationMessage.OrdersNotEmpty);

            // Nested validation for BatchOrdersCreate
            RuleForEach(x => x.Orders).SetValidator(new BatchOrdersCreateValidator());
        }
    }

    public class BatchOrdersCreateValidator : AbstractValidator<BatchOrdersCreate>
    {
        public BatchOrdersCreateValidator()
        {
            // Rule for Id: must be greater than 0 if provided
            RuleFor(x => x.Id)
                .GreaterThan(0).When(x => x.Id.HasValue)
                .WithMessage(ValidationMessage.OrderIdGreaterThanZero);
        }
    }
}
