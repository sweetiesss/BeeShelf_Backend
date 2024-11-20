namespace BeeStore_Repository.Data
{
    public class AppConfiguration
    {
        public string DatabaseConnection { get; set; } = string.Empty;
        public string KeyVaultURL { get; set; }
        public string sourceMail { get; set; }
        public string PaymentRequest { get; set; }
    }
}
