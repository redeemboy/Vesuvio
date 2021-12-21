using Vesuvio.Core;
using Vesuvio.Domain.DTO;
using Vesuvio.Domain.Model;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vesuvio.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private UserManager<ApplicationUser> _userManager;
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly Guid _userId;

        public UserService(IUnitOfWork uow, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _userManager = userManager;
            _claimsPrincipal = httpContextAccessor.HttpContext.User;
            if (_claimsPrincipal.Claims.Where(x => x.Type == "id").Count() > 0)
            {
                _userId = Guid.Parse(_claimsPrincipal.Claims.Where(x => x.Type == "id").FirstOrDefault().Value);
            }
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            var result = (from userrole in _uow.GetRepository<IdentityUserRole<Guid>>().GetQuerable()
                          join users in _uow.GetRepository<ApplicationUser>().GetQuerable() on userrole.UserId equals users.Id
                          join role in _uow.GetRepository<ApplicationRole>().GetQuerable() on userrole.RoleId equals role.Id
                          where users.DeletedDate == null
                          select new
                          {
                              UserId = users.Id,
                              FirstName = users.FirstName,
                              MiddleName = users.MiddleName,
                              LastName = users.LastName,
                              Email = users.Email,
                              Post = users.Post,
                              PhoneNumber = users.PhoneNumber,
                              UserName = users.UserName,
                              RoleId = role.Id,
                              RoleName = role.Name
                          }).AsEnumerable().GroupBy(q => q.UserId).Select(q => new UserDTO
                          {
                              Id = q.Key,
                              FirstName = q.FirstOrDefault().FirstName,
                              MiddleName = q.FirstOrDefault().MiddleName,
                              LastName = q.FirstOrDefault().LastName,
                              Email = q.FirstOrDefault().Email,
                              Post = q.FirstOrDefault().Post,
                              PhoneNumber = q.FirstOrDefault().PhoneNumber,
                              UserName = q.FirstOrDefault().UserName,
                              Roles = q.Select(a => a.RoleName).ToList()
                          });

            return result;

        }

        public UserDTO GetUserById(Guid Id)
        {

            var result = (from userrole in _uow.GetRepository<IdentityUserRole<Guid>>().GetQuerable(x => x.UserId == Id)
                          join users in _uow.GetRepository<ApplicationUser>().GetQuerable(x => x.Id == Id) on userrole.UserId equals users.Id
                          join role in _uow.GetRepository<ApplicationRole>().GetQuerable() on userrole.RoleId equals role.Id
                          select new
                          {
                              UserId = users.Id,
                              FirstName = users.FirstName,
                              MiddleName = users.MiddleName,
                              LastName = users.LastName,
                              Email = users.Email,
                              Post = users.Post,
                              PhoneNumber = users.PhoneNumber,
                              UserName = users.UserName,
                              RoleId = role.Id,
                              RoleName = role.Name,
                              ImageUrl = users.ImageUrl,


                          }).AsEnumerable().GroupBy(q => q.UserId).Select(q => new UserDTO
                          {
                              Id = q.Key,
                              FirstName = q.FirstOrDefault().FirstName,
                              MiddleName = q.FirstOrDefault().MiddleName,
                              LastName = q.FirstOrDefault().LastName,
                              Email = q.FirstOrDefault().Email,
                              Post = q.FirstOrDefault().Post,
                              PhoneNumber = q.FirstOrDefault().PhoneNumber,
                              UserName = q.FirstOrDefault().UserName,
                              ImageUrl = q.FirstOrDefault().ImageUrl,
                              Roles = q.Select(a => a.RoleName).ToList(),
                          });
            return result.FirstOrDefault();

        }

        public IQueryable<UserDTO> GetUsersByRole(Guid RoleId)
        {

            var result = (from userrole in _uow.GetRepository<IdentityUserRole<Guid>>().GetQuerable(x => x.RoleId == RoleId)
                          join users in _uow.GetRepository<ApplicationUser>().GetQuerable() on userrole.UserId equals users.Id
                          join role in _uow.GetRepository<ApplicationRole>().Get(x => x.Id == RoleId) on userrole.RoleId equals role.Id
                          select new
                          {
                              UserId = users.Id,
                              FirstName = users.FirstName,
                              MiddleName = users.MiddleName,
                              LastName = users.LastName,
                              Email = users.Email,
                              Post = users.Post,
                              PhoneNumber = users.PhoneNumber,
                              UserName = users.UserName,
                              RoleId = role.Id,
                              RoleName = role.Name
                          }).GroupBy(q => q.UserId).Select(q => new UserDTO
                          {
                              Id = q.Key,
                              FirstName = q.FirstOrDefault().FirstName,
                              MiddleName = q.FirstOrDefault().MiddleName,
                              LastName = q.FirstOrDefault().LastName,
                              Email = q.FirstOrDefault().Email,
                              Post = q.FirstOrDefault().Post,
                              PhoneNumber = q.FirstOrDefault().PhoneNumber,
                              UserName = q.FirstOrDefault().UserName,
                              Roles = q.Select(a => a.RoleName).ToList()
                          });
            return result;
        }

        public IEnumerable<UserDTO> SearchUser(DataTableRequest request, List<UserDTO> userData)
        {
            IEnumerable<UserDTO> filteredAppUserData;
            if (request.Search != null && request.Search.Value != string.Empty)
            {
                var searchText = request.Search.Value.ToLower().Trim();
                filteredAppUserData = userData.Where(p =>
                        p.FirstName.ToLower().Contains(searchText) ||
                        p.LastName.ToString().ToLower().Contains(searchText) ||
                        p.Email.ToLower().Contains(searchText) ||
                        p.UserName.ToLower().Contains(searchText));
            }
            else
            {
                filteredAppUserData = userData;
            }
            return SortUser(request, filteredAppUserData);
        }

        public IEnumerable<UserDTO> SortUser(DataTableRequest request, IEnumerable<UserDTO> filteredAppUserData)
        {
            if (request.Order != null && request.Order.Any())
            {
                string sortColumn = request.Order[0].Column;
                string sortDirection = request.Order[0].Dir;

                Func<UserDTO, string> userFunctionString = null;

                switch (sortColumn.ToLower())
                {
                    case "firstname":
                        {
                            userFunctionString = (c => c.FirstName);

                            break;
                        }

                    case "email":
                        {
                            userFunctionString = (c => c.Email);

                            break;
                        }
                    case "post":
                        {
                            userFunctionString = (c => c.Post);

                            break;
                        }
                    case "phonenumber":
                        {
                            userFunctionString = (c => c.PhoneNumber);

                            break;
                        }
                    case "username":
                        {
                            userFunctionString = (c => c.UserName);

                            break;
                        }
                }

                filteredAppUserData =
                                sortDirection == "asc"
                                    ? filteredAppUserData.OrderBy(userFunctionString).AsQueryable()
                                    : filteredAppUserData.OrderByDescending(userFunctionString).AsQueryable();
            }
            return filteredAppUserData;
        }

        public IEnumerable<UserDTO> PaginationUser(DataTableRequest request, IEnumerable<UserDTO> filteredAppUserData)
        {
            return filteredAppUserData.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
        }

        public Guid GetCurrentUserId()
        {
            Guid userId = Guid.Parse(_claimsPrincipal.Claims.Where(x => x.Type == "id").FirstOrDefault().Value);
            return userId;
        }

        public CustomerOrders GetCustomerOrders(Guid userId)
        {
            List<Order> orders = _uow.GetRepository<Order>().Get(q => q.DeletedDate == null && q.BuyerId == userId).OrderByDescending(q => q.OrderDate).ToList();
            ApplicationUser user = _uow.GetRepository<ApplicationUser>().Get(q => q.Id == userId && q.DeletedDate == null).FirstOrDefault();

            if (user != null)
            {
                CustomerUser customerUser = new CustomerUser().MapToCustomerDTO(user);
                return new CustomerOrders
                {
                    CustomerUser = customerUser,
                    Orders = (orders.Count() > 0) ? orders.Select(order => new OrderDTO().MapToOrderDTO(order)).ToList() : null

                };
            }

            return null;
        }

        public async Task<string> ChangePassword(PasswordChangeDTO dto)
        {
            ApplicationUser user = _uow.GetRepository<ApplicationUser>().Get(q => q.Id == _userId && q.DeletedDate == null).FirstOrDefault();

            // check the credentials
            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return await Task.FromResult<string>(Constants.ErrorCodes.InvalidCredentials);
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (result.Succeeded)
            {
                return "success";
            }

            return "error";
        }

        public async Task<IdentityResult> CreateUser(UserDTO userDTO)
        {
            ApplicationUser entity = new ApplicationUser();
            entity = userDTO.MapToUser(userDTO, entity);
            entity.CreatedDate = DateTime.Now;
            entity.CreatedBy = _userId;
            IdentityResult result = await _userManager.CreateAsync(entity, userDTO.Password);
            if (result.Succeeded)
                result = await _userManager.AddToRolesAsync(entity, userDTO.Roles);

            return result;
        }

        public async Task<IdentityResult> UpdateUser(UserDTO userDTO)
        {
            var updateUserEntity = _uow.GetRepository<ApplicationUser>().Get(x => x.Id == userDTO.Id).FirstOrDefault();
            IGenericRepository<ApplicationRole> roleRepository = _uow.GetRepository<ApplicationRole>();
            updateUserEntity = userDTO.MapToUser(userDTO, updateUserEntity);
            updateUserEntity.ModifiedBy = _userId;
            updateUserEntity.ModifiedDate = DateTime.Now;
            IdentityResult result = await _userManager.UpdateAsync(updateUserEntity);

            if (userDTO.Roles != null && userDTO.Roles.Count > 0)
            {
                var roles = await _userManager.GetRolesAsync(updateUserEntity);
                if (roles.Count > 0)
                {
                    await _userManager.RemoveFromRolesAsync(updateUserEntity, roles);
                }

                result = await _userManager.AddToRolesAsync(updateUserEntity, userDTO.Roles);
            }
            return result;
        }

        public async Task<IdentityResult> AddClaimAsync(ApplicationUser userIdentity, string roleName)
        {
            return await _userManager.AddClaimAsync(userIdentity, new Claim(ClaimTypes.Role, roleName));
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser userIdentity, string roleName)
        {

            var result = await _userManager.AddToRoleAsync(userIdentity, roleName);
            if (result.Succeeded) await _uow.SaveChangesAsync();
            return result;

        }

        public bool VerifyCustomer(UserDTO dto)
        {
            string role = _claimsPrincipal.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault().Value;
            if (role == Constants.Operation.Customer)
            {
                if (dto.Id == _userId)
                {
                    return true;
                }

                return false;
            }

            return true;
        }
        public bool DeleteUser(List<Guid> ids)
        {
            IGenericRepository<ApplicationUser> userRepository = _uow.GetRepository<ApplicationUser>();
            foreach (var id in ids)
            {
                ApplicationUser user = userRepository.Get(q => q.Id == id).FirstOrDefault();
                user.DeletedDate = DateTime.Now;
                user.ModifiedBy = _userId;

                userRepository.Update(user);
            }

            return SaveData();
        }

        private bool SaveData()
        {
            var result = this._uow.SaveChanges();
            return result >= 1 ? true : false;
        }
    }
}
