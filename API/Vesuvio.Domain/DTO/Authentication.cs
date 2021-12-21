using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Vesuvio.Domain.DTO
{
    public class SigninDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class SignupDTO : SigninDTO
    {
        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Post { get; set; }

        //[Required]
        public string PhoneNumber { get; set; }

        public List<string> Roles { get; set; }
    }
}
