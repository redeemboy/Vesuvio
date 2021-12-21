using Vesuvio.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Vesuvio.Domain.DTO
{
    public class UserDTO : SignupDTO
    {
        public Guid? Id { get; set; }
		
        public string ImageUrl { get; set; }

        public bool RemoveImage { get; set; }


        public ApplicationUser MapToUser(UserDTO dto, ApplicationUser appUser)
        {
            appUser.FirstName = dto.FirstName;
            appUser.MiddleName = dto.MiddleName;
            appUser.LastName = dto.LastName;
            appUser.Post = dto.Post;
            appUser.PhoneNumber = dto.PhoneNumber;
            appUser.ImageUrl = dto.ImageUrl;
            if (dto.Id.HasValue)
            {
                appUser.Id = dto.Id.Value;
            }
            else
            {
                appUser.Email = dto.Email;
                appUser.UserName = dto.UserName;
            }
            return appUser;
        }
    }

    public class ModelUser
    {
        public Guid? UserId { get; set; }

        public string UserName { get; set; }
    }

    public class PasswordChangeDTO
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

    public class CustomerOrders
    {
        public CustomerUser CustomerUser { get; set; }

        public List<OrderDTO> Orders { get; set; }
    }

    public class CustomerUser : SignupDTO
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }

        public CustomerUser MapToCustomerDTO(ApplicationUser customerUser)
        {
            CustomerUser dto = new CustomerUser();

            dto.Name = customerUser.FirstName + string.Empty + customerUser.MiddleName + string.Empty + customerUser.LastName;
            dto.FirstName = customerUser.FirstName;
            dto.MiddleName = customerUser.MiddleName;
            dto.LastName = customerUser.LastName;
            dto.UserName = customerUser.UserName;
            dto.Email = customerUser.Email;
            dto.PhoneNumber = customerUser.PhoneNumber;

            return dto;
        }
    }

}
