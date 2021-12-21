using Vesuvio.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vesuvio.Domain.DTO
{
    public class RoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public ApplicationRole MapToRole(RoleDTO dto, ApplicationRole appRole = null)
        {
            if(appRole == null)
            {
                return new ApplicationRole()
                {
                   
                    Name = dto.Name,
                    NormalizedName = dto.NormalizedName
                };

            }

            appRole.Id = dto.Id;
            appRole.Name = dto.Name;
            appRole.NormalizedName = dto.NormalizedName;

            return appRole;
        }

        public RoleDTO MapToRoleDTO(ApplicationRole appRole)
        {
            RoleDTO dto = new RoleDTO();

            dto.Name = appRole.Name;
            dto.Id = appRole.Id;
            dto.NormalizedName = appRole.NormalizedName;

            return dto;
        }

        public List<RoleDTO> MapToRolesDTO(List<ApplicationRole> appRoles)
        {
            List<RoleDTO> dtos = new List<RoleDTO>();

            foreach (ApplicationRole role in appRoles)
            {
                dtos.Add(MapToRoleDTO(role));
            }

            return dtos;
        }
    }

    
}
