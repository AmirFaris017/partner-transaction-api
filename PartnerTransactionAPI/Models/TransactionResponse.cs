using System.Text.Json.Serialization;

namespace PartnerTransactionAPI.Models
{
    public class TransactionResponse
    {
        // 1 = success, 0 = failure
        [JsonPropertyName("result")]
        public int Result { get; set; }

        // Total amount submitted (only on success)
        [JsonPropertyName("totalamount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? TotalAmount { get; set; }

        // Discount amount in cents (only on success)
        [JsonPropertyName("totaldiscount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? TotalDiscount { get; set; }

        // Final amount after discount (only on success)
        [JsonPropertyName("finalamount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? FinalAmount { get; set; }

        // Error message (only on failure)
        [JsonPropertyName("resultmessage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ResultMessage { get; set; }

        
        public static TransactionResponse Success(long totalAmount, long totalDiscount, long finalAmount)
        {
            return new TransactionResponse
            {
                Result = 1,
                TotalAmount = totalAmount,
                TotalDiscount = totalDiscount,
                FinalAmount = finalAmount
            };
        }

        public static TransactionResponse Failure(string message)
        {
            return new TransactionResponse
            {
                Result = 0,
                ResultMessage = message
            };
        }
    }
}