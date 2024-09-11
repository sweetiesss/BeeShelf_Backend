using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Data
{
    public class AppConfiguration
    {
        public string DatabaseConnection { get; set; } = "server=beestoredatabase.cdciasgw89n8.ap-northeast-1.rds.amazonaws.com;port=3306;user=user;password=baijainonsoaunguqnwunqwumdoaj;database=BeestoreDb";
    }
}
