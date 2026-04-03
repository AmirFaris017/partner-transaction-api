using FluentValidation;
using PartnerTransactionAPI.Models;

namespace PartnerTransactionAPI.Validators
{
    public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
    {
        public TransactionRequestValidator()
        {
            // partnerkey — mandatory, max 50 chars
            RuleFor(x => x.PartnerKey)
                .NotEmpty()
                .WithMessage("partnerkey is Required.")
                .MaximumLength(50)
                .WithMessage("partnerkey must not exceed 50 characters.");

            // partnerrefno — mandatory, max 50 chars
            RuleFor(x => x.PartnerRefNo)
                .NotEmpty()
                .WithMessage("partnerrefno is Required.")
                .MaximumLength(50)
                .WithMessage("partnerrefno must not exceed 50 characters.");

            // partnerpassword — mandatory, max 50 chars
            RuleFor(x => x.PartnerPassword)
                .NotEmpty()
                .WithMessage("partnerpassword is Required.")
                .MaximumLength(50)
                .WithMessage("partnerpassword must not exceed 50 characters.");

            // totalamount — mandatory, must be positive
            RuleFor(x => x.TotalAmount)
                .NotNull()
                .WithMessage("totalamount is Required.")
                .GreaterThan(0)
                .WithMessage("totalamount must be a positive value.");

            // timestamp — mandatory, must be valid ISO 8601
            RuleFor(x => x.Timestamp)
                .NotEmpty()
                .WithMessage("timestamp is Required.")
                .Must(ts => DateTime.TryParse(ts, null,
                    System.Globalization.DateTimeStyles.RoundtripKind, out _))
                .WithMessage("timestamp format is invalid.");

            // sig — mandatory
            RuleFor(x => x.Sig)
                .NotEmpty()
                .WithMessage("sig is Required.");

            // items — optional but if provided, validate each item
            RuleForEach(x => x.Items)
                .SetValidator(new ItemDetailValidator())
                .When(x => x.Items != null && x.Items.Count > 0);
        }
    }
}