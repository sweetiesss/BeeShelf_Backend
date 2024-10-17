using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Data
{
    public class AppConfiguration
    {
        public string DatabaseConnection { get; set; } = string.Empty;
        public string KeyVaultURL { get; set; }
        public string sourceMail {  get; set; }
    }
}
