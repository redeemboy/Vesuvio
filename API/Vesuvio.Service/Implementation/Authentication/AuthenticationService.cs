using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Vesuvio.Core;
using Vesuvio.Domain.Model;
using Vesuvio.Domain.DTO;
using System.Security.Claims;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Transactions;

namespace Vesuvio.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public AuthenticationService(IUnitOfWork uow, UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _uow = uow;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<IdentityResult> SignUp(SignupDTO signupUser)
        {
            var appUser = new ApplicationUser()
            {
                Email = signupUser.Email,
                FirstName = signupUser.FirstName,
                MiddleName = signupUser.MiddleName,
                LastName = signupUser.LastName,
                UserName = signupUser.UserName ?? signupUser.Email,
                PhoneNumber = signupUser.PhoneNumber
            };

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    ApplicationUser existingUser = FindUserAcync(appUser.UserName);
                    IdentityResult result = new IdentityResult();
                    if (existingUser == null)
                    {
                        result = await _userManager.CreateAsync(appUser, signupUser.Password);
                        if (result.Succeeded)
                        {
                            signupUser.Roles = new List<string>();
                            signupUser.Roles.Add(Constants.Operation.Customer);
                            result = await _userManager.AddToRolesAsync(appUser, signupUser.Roles);
                        }

                        scope.Complete();
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
        }

        private ApplicationUser FindUserAcync(string loginUser)
        {
            var result = _uow.GetRepository<ApplicationUser>().GetQuerable().Where(q => (q.Email == loginUser || q.UserName == loginUser) && q.DeletedDate == null).FirstOrDefault();
            return result;
        }

        public async Task<string> Authentication(string loginUser, string password)
        {
            string returnString = await Task.FromResult<string>(Constants.ErrorCodes.InvalidCredentials);
            if (!(string.IsNullOrEmpty(loginUser) || string.IsNullOrEmpty(password)))
            {
                // get the user to verify
                var userToVerify = FindUserAcync(loginUser);

                if (userToVerify != null)
                {
                    IList<string> userRolesList = await _userManager.GetRolesAsync(userToVerify);

                    if (userRolesList.Count() == 0)
                    {
                        returnString = await Task.FromResult<string>(Constants.ErrorCodes.UnAuthorized);
                    }
                    // check the credentials
                    if (await _userManager.CheckPasswordAsync(userToVerify, password))
                    {
                        returnString = await _jwtService.GenerateJwt(userToVerify, userRolesList);
                    }
                }
            }

            return returnString;
        }
    }
}
