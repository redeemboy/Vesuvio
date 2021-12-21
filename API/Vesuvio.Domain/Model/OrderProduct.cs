using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vesuvio.Domain.Model
{
    public class OrderProduct : CommonModel
    {

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [ForeignKey("Order")]
        public Guid? OrderId { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
