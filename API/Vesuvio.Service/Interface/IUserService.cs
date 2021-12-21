using Vesuvio.Domain.DTO;
using Vesuvio.Domain.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vesuvio.Service
{
    public interface IUserService
    {

        Guid GetCurrentUserId();

        IEnumerable<UserDTO> GetAllUsers();

        IEnumerable<UserDTO> SearchUser(DataTableRequest request, List<UserDTO> userData);

        IEnumerable<UserDTO> SortUser(DataTableRequest request, IEnumerable<UserDTO> filteredAppUserData);

        IEnumerable<UserDTO> PaginationUser(DataTableRequest request, IEnumerable<UserDTO> filteredAppUserData);

        Task<IdentityResult> CreateUser(UserDTO userDTO);

        Task<IdentityResult> UpdateUser(UserDTO userDTO);

        bool DeleteUser(List<Guid> ids);

        IQueryable<UserDTO> GetUsersByRole(Guid RoleId);

        UserDTO GetUserById(Guid Id);

        CustomerOrders GetCustomerOrders(Guid userId);

        Task<string> ChangePassword(PasswordChangeDTO dto);
        bool VerifyCustomer(UserDTO dto);
    }
}
