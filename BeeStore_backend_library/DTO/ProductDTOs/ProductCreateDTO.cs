using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.ProductDTOs
{
    public class ProductCreateDTO
    {

        public string PartnerEmail { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Weight { get; set; }

        public DateTime? CreateDate { get; set; }

        public int ProductAmount { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int PictureId { get; set; }

        public int ProductCategoryId { get; set; }

        public string Origin { get; set; }
    }
}
