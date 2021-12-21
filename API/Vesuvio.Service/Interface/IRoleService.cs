using Vesuvio.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vesuvio.Service
{
    public interface IRoleService
    {
        List<RoleDTO> GetRoles();

        List<RoleDTO> GetAllRoles(DataTableRequest request);

        bool CreateUpdateRole(RoleDTO dto, string operation);

        bool DeleteRole(Guid id);

    }
}
