using System;
using System.Security.Cryptography;
using System.Text;

namespace PartnerTransactionAPI.Services
{
    public class SignatureService
    {
        public string GenerateSignature(string partnerKey, string partnerRefNo,string partnerPassword, long totalAmount,string timestamp)
        {
            DateTime parsedTime = DateTime.Parse(timestamp, null, 
                                  System.Globalization.DateTimeStyles.RoundtripKind);
            string sigTimestamp = parsedTime.ToString("yyyyMMddHHmmss");

            string rawString = $"{sigTimestamp}{partnerKey}{partnerRefNo}{totalAmount}{partnerPassword}";

            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawString));
            string hexHash = Convert.ToHexString(hashBytes).ToLower();

            string base64Sig = Convert.ToBase64String(Encoding.UTF8.GetBytes(hexHash));

            return base64Sig;
        }

        public bool ValidateSignature(string partnerKey, string partnerRefNo,
                                      string partnerPassword, long totalAmount,
                                      string timestamp, string incomingSig)
        {
            // Generate what the signature SHOULD be
            string expectedSig = GenerateSignature(partnerKey, partnerRefNo, 
                                                   partnerPassword, totalAmount, 
                                                   timestamp);

            // Compare with what was sent — must match exactly
            return string.Equals(expectedSig, incomingSig, StringComparison.Ordinal);
        }
    }
}