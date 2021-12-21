using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vesuvio.Domain.Model
{
    public class Order : CommonModel
    {

        [StringLength(500)]
        public string OrderNumber { get; set; }

        [ForeignKey("Buyer")]
        public Guid? BuyerId { get; set; }

        public virtual ApplicationUser Buyer { get; set; }

        public DateTime OrderDate { get; set; }

        [StringLength(10)]
        public string Status { get; set; }


        [StringLength(500)]
        public string Caption { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
