using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BeeStore_Repository.DTO.ProductDTOs
{
    public class ProductListDTO
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public decimal? Weight { get; set; }

        public DateTime? CreateDate { get; set; }

        public int? ProductAmount { get; set; }

        [JsonIgnore]
        public DateTime? ExpirationDate { get; set; }   //THIS SHOULD NOT BE HERE

        public string Picture_Link { get; set; }
        //public int? PictureId { get; set; }

        public string ProductCategoryName { get; set; }
        //public int? ProductCategoryId { get; set; }

        public string? Origin { get; set; }

    }
}
