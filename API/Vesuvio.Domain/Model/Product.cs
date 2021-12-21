using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vesuvio.Domain.Model
{
    public class Product : CommonModel
    {
        [StringLength(8)]
        public string SKU { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string FinishedProductSize { get; set; }

        [StringLength(255)]
        public string FinishedProductWeight { get; set; }

        [StringLength(255)]
        public string AvailableMaterial { get; set; }

        [StringLength(255)]
        public string AvailableColor { get; set; }

        [StringLength(255)]
        public string Price { get; set; }

        [StringLength(255)]
        public string LeadTime { get; set; }

        //We can use this field to target specific customer. Value will be multiple value seperated by comma.
        [StringLength(255)]
        public string Tag { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }
    }
}