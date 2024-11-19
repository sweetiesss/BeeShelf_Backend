namespace BeeStore_Repository.Data
{
    public class AppConfiguration
    {
        public string DatabaseConnection { get; set; } = string.Empty;
        public string KeyVaultURL { get; set; }
        public string sourceMail { get; set; }

        public string ClientID { get; set; }
        public string APIKey { get; set; }
        public string ChecksumKey { get; set; }
        public string POSTpaymentRequest { get; set; }
        public string GETpaymentInfo { get; set; }
    }
}
