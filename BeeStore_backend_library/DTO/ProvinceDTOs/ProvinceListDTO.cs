using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.ProvinceDTOs
{
    public class ProvinceListDTO
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public string? SubDivisionName { get; set; }

        public string? SubDivsisionCategory { get; set; }
    }
}
