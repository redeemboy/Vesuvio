using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vesuvio.Domain.Model
{
    public class CommonModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("CreatedByUser")]
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("DeletedByUser")]
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public virtual ApplicationUser CreatedByUser { get; set; }
        public virtual ApplicationUser ModifiedByUser { get; set; }
        public virtual ApplicationUser DeletedByUser { get; set; }
    }
}