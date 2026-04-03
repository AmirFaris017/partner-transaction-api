using FluentValidation.TestHelper;
using PartnerTransactionAPI.Models;
using PartnerTransactionAPI.Validators;
using Xunit;

namespace PartnerTransactionAPI.Tests
{
    public class TransactionRequestValidatorTests
    {
        private readonly TransactionRequestValidator _validator =
            new TransactionRequestValidator();

        private TransactionRequest ValidRequest() => new TransactionRequest
        {
            PartnerKey      = "FAKEGOOGLE",
            PartnerRefNo    = "FG-00001",
            PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
            TotalAmount     = 1000,
            Timestamp       = DateTime.UtcNow.ToString("o"),
            Sig             = "somesig"
        };

        [Fact]
        public void Validate_ValidRequest_NoErrors()
        {
            var result = _validator.Validate(ValidRequest());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_MissingPartnerKey_ReturnsError()
        {
            var request = ValidRequest();
            request.PartnerKey = null;

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.PartnerKey)
                  .WithErrorMessage("partnerkey is Required.");
        }

        [Fact]
        public void Validate_NegativeTotalAmount_ReturnsError()
        {
            var request = ValidRequest();
            request.TotalAmount = -100;

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.TotalAmount)
                  .WithErrorMessage("totalamount must be a positive value.");
        }

        [Fact]
        public void Validate_InvalidTimestamp_ReturnsError()
        {
            var request = ValidRequest();
            request.Timestamp = "not-a-valid-date";

            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Timestamp)
                  .WithErrorMessage("timestamp format is invalid.");
        }

        [Fact]
        public void Validate_ItemQtyExceedsFive_ReturnsError()
        {
            var request = ValidRequest();
            request.Items = new List<ItemDetail>
            {
                new() {
                    PartnerItemRef = "i-001",
                    Name      = "Pen",
                    Qty       = 6,   // exceeds max of 5
                    UnitPrice = 100
                }
            };

            var result = _validator.TestValidate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.ErrorMessage == "qty must not exceed 5.");
        }

        [Fact]
        public void Validate_MissingItemName_ReturnsError()
        {
            var request = ValidRequest();
            request.Items = new List<ItemDetail>
            {
                new() {
                    PartnerItemRef = "i-001",
                    Name      = "",  // empty name
                    Qty       = 1,
                    UnitPrice = 100
                }
            };

            var result = _validator.TestValidate(request);
            Assert.Contains(result.Errors,
                e => e.ErrorMessage == "name is Required.");
        }
    }
}