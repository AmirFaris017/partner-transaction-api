using PartnerTransactionAPI.Services;
using Xunit;

namespace PartnerTransactionAPI.Tests
{
    public class SignatureServiceTests
    {
        private readonly SignatureService _sut = new SignatureService();

        [Fact]
        public void GenerateSignature_AssessmentSample_ReturnsExpectedSig()
        {
            string partnerKey      = "FAKEGOOGLE";
            string partnerRefNo    = "FG-00001";
            string partnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==";
            long   totalAmount     = 1000;
            string timestamp      = "2024-08-15T02:11:22.0000000Z";
            string expectedSig    = "MDE3ZTBkODg4ZDNhYzU0ZDBlZWRmNmU2NmUyOWRhZWU4Y2M1NzQ1OTIzZGRjYTc1ZGNjOTkwYzg2MWJlMDExMw==";

            // Act
            string result = _sut.GenerateSignature(
                partnerKey, partnerRefNo, partnerPassword, totalAmount, timestamp);

            // Assert
            Assert.Equal(expectedSig, result);
        }

        [Fact]
        public void ValidateSignature_CorrectSignature_ReturnsTrue()
        {
            string partnerKey      = "FAKEGOOGLE";
            string partnerRefNo    = "FG-00001";
            string partnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==";
            long   totalAmount     = 1000;
            string timestamp      = "2024-08-15T02:11:22.0000000Z";
            string sig            = "MDE3ZTBkODg4ZDNhYzU0ZDBlZWRmNmU2NmUyOWRhZWU4Y2M1NzQ1OTIzZGRjYTc1ZGNjOTkwYzg2MWJlMDExMw==";

            bool result = _sut.ValidateSignature(
                partnerKey, partnerRefNo, partnerPassword, totalAmount, timestamp, sig);

            Assert.True(result);
        }

        [Fact]
        public void ValidateSignature_WrongSignature_ReturnsFalse()
        {
            bool result = _sut.ValidateSignature(
                "FAKEGOOGLE", "FG-00001", "RkFLRVBBU1NXT1JEMTIzNA==",
                1000, "2024-08-15T02:11:22.0000000Z",
                "wrongsignature");

            Assert.False(result);
        }

        [Fact]
        public void ValidateSignature_TamperedAmount_ReturnsFalse()
        {
            // Sig was generated for amount 1000 but we send 9999
            string sig = "MDE3ZTBkODg4ZDNhYzU0ZDBlZWRmNmU2NmUyOWRhZWU4Y2M1NzQ1OTIzZGRjYTc1ZGNjOTkwYzg2MWJlMDExMw==";

            bool result = _sut.ValidateSignature(
                "FAKEGOOGLE", "FG-00001", "RkFLRVBBU1NXT1JEMTIzNA==",
                9999, "2024-08-15T02:11:22.0000000Z", sig);

            Assert.False(result);
        }
    }
}