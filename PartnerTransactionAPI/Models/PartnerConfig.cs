namespace PartnerTransactionAPI.Models
{
    // Represents one allowed partner entry
    public class PartnerConfig
    {
        public string PartnerNo { get; set; } = string.Empty;
        public string PartnerKey { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}