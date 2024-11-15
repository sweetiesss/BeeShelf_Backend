using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.PartnerDTOs
{
    public class PartnerRevenueDTO
    {
        public string? orderStatus {  get; set; }
        public int orderAmount {  get; set; } = 0;
        public int amount { get; set; } = 0;


    }
}
