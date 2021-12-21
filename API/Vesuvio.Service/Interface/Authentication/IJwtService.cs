using Vesuvio.Domain.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Vesuvio.Service
{
    public interface IJwtService
    {
        //ClaimsIdentity GenerateClaimsIdentity(string userName, string id, IList<string> roles);

        Task<string> GenerateJwt(ApplicationUser user, IList<string> roles);
    }
}
