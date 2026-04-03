using FluentValidation;
using PartnerTransactionAPI.Models;

namespace PartnerTransactionAPI.Validators
{
    public class ItemDetailValidator : AbstractValidator<ItemDetail>
    {
        public ItemDetailValidator()
        {
            // partneritemref — mandatory, not empty
            RuleFor(x => x.PartnerItemRef)
                .NotEmpty()
                .WithMessage("partneritemref is Required.")
                .MaximumLength(50)
                .WithMessage("partneritemref must not exceed 50 characters.");

            // name — mandatory, not empty, max 100 chars
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("name is Required.")
                .MaximumLength(100)
                .WithMessage("name must not exceed 100 characters.");

            // qty — mandatory, positive, max 5
            RuleFor(x => x.Qty)
                .NotNull()
                .WithMessage("qty is Required.")
                .GreaterThan(0)
                .WithMessage("qty must be a positive value.")
                .LessThanOrEqualTo(5)
                .WithMessage("qty must not exceed 5.");

            // unitprice — mandatory, positive
            RuleFor(x => x.UnitPrice)
                .NotNull()
                .WithMessage("unitprice is Required.")
                .GreaterThan(0)
                .WithMessage("unitprice must be a positive value.");
        }
    }
}