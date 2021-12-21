using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vesuvio.Domain.Model
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Post { get; set; }

        [StringLength(5000)]
        public string Settings { get; set; }

        [StringLength(100)]
        public string ImageUrl { get; set; }

        [ForeignKey("CreatedByUser")]
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("DeletedByUser")]
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

    }

    public class ApplicationRole : IdentityRole<Guid>
    {
    }
}
