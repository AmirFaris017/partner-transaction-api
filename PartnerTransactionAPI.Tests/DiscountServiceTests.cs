using PartnerTransactionAPI.Services;
using Xunit;

namespace PartnerTransactionAPI.Tests
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _sut = new DiscountService();
        // "sut" = System Under Test — standard naming convention

        [Fact]
        public void Calculate_AmountBelow200_NoDiscount()
        {
            // Arrange — MYR 150 = 15000 cents
            long totalAmountCents = 15000;

            // Act
            var (discount, finalAmount) = _sut.Calculate(totalAmountCents);

            // Assert
            Assert.Equal(0, discount);
            Assert.Equal(15000, finalAmount);
        }

        [Fact]
        public void Calculate_AmountBetween200And500_5PercentDiscount()
        {
            // MYR 500 = 50000 cents → 5% discount = 2500
            var (discount, finalAmount) = _sut.Calculate(50000);

            Assert.Equal(2500, discount);
            Assert.Equal(47500, finalAmount);
        }

        [Fact]
        public void Calculate_AmountBetween501And800_7PercentDiscount()
        {
            // MYR 800 = 80000 cents → 7% discount = 5600
            var (discount, finalAmount) = _sut.Calculate(80000);

            Assert.Equal(5600, discount);
            Assert.Equal(74400, finalAmount);
        }

        [Fact]
        public void Calculate_AmountBetween801And1200_10PercentDiscount()
        {
            // MYR 1000 = 100000 cents → 10% = 10000
            var (discount, finalAmount) = _sut.Calculate(100000);

            Assert.Equal(10000, discount);
            Assert.Equal(90000, finalAmount);
        }

        [Fact]
        public void Calculate_AmountAbove1200_15PercentDiscount()
        {
            // MYR 2000 = 200000 cents → 15% = 30000
            var (discount, finalAmount) = _sut.Calculate(200000);

            Assert.Equal(30000, discount);
            Assert.Equal(170000, finalAmount);
        }

        [Fact]
        public void Calculate_TotalDiscount_CapAt20Percent()
        {
            // MYR 1205 = 120500 cents
            // Base: 15% (above 1200) + Conditional: 10% (ends in 5, above 900)
            // Total 25% → capped at 20% → discount = 24100
            var (discount, finalAmount) = _sut.Calculate(120500);

            Assert.Equal(24100, discount);
            Assert.Equal(96400, finalAmount);
        }

        [Fact]
        public void Calculate_PrimeAmountAbove500_AdditionalEightPercent()
        {
            // MYR 503 is prime and above 500
            // Base: 7% (501-800) + Conditional: 8% (prime) = 15%
            // 50300 * 15% = 7545
            var (discount, _) = _sut.Calculate(50300);

            Assert.Equal(7545, discount);
        }

        [Fact]
        public void Calculate_EndsIn5Above900_AdditionalTenPercent()
        {
            // MYR 1005 ends in 5 and is above 900
            // Base: 10% (801-1200) + Conditional: 10% (ends in 5) = 20%
            // 100500 * 20% = 20100
            var (discount, _) = _sut.Calculate(100500);

            Assert.Equal(20100, discount);
        }

        [Theory]
        [InlineData(120500, 24100, 96400)]   // assessment sample 2
        [InlineData(100000, 10000, 90000)]   // assessment sample 1
        [InlineData(15000,  0,     15000)]    // below MYR 200
        public void Calculate_AssessmentSamples_MatchExpected(
            long input, long expectedDiscount, long expectedFinal)
        {
            var (discount, finalAmount) = _sut.Calculate(input);

            Assert.Equal(expectedDiscount, discount);
            Assert.Equal(expectedFinal, finalAmount);
        }
    }
}