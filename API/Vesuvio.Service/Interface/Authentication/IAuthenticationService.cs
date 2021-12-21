using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Vesuvio.Domain.DTO;

namespace Vesuvio.Service
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> SignUp(SignupDTO singupUser);

        Task<string> Authentication(string userName, string password);
    }
}
