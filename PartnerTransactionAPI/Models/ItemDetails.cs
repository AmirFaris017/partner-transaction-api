using System.Text.Json.Serialization;

namespace PartnerTransactionAPI.Models
{
    public class ItemDetail
    {
        [JsonPropertyName("partneritemref")]
        public string? PartnerItemRef { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("qty")]
        public int? Qty { get; set; }

        [JsonPropertyName("unitprice")]
        public long? UnitPrice { get; set; }
    }
}