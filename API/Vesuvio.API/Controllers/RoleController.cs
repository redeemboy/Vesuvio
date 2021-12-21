using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vesuvio.Domain.DTO;
using Vesuvio.Service;

namespace Vesuvio.API.Controllers
{
    public class RoleController : BaseController
    {

        private readonly IRoleService _roleService;
        private readonly IAuthenticationService _authenticationService;


        public RoleController(IRoleService roleService, IAuthenticationService authenticationService)
        {
            _roleService = roleService;
            _authenticationService = authenticationService;
        }

        [HttpGet("getall")]
        [Authorize(Policy = "Admin")]
        public IActionResult Get()
        {
            List<RoleDTO> result = _roleService.GetRoles();
            return Ok(result);
        }

        [HttpPost("create")]
        [Authorize(Policy = "Admin")]
        public IActionResult Post(RoleDTO role)
        {
            var result = _roleService.CreateUpdateRole(role, Constants.Operation.Insert);
            return Ok(result);
        }

        [HttpPut("update")]
        [Authorize(Policy = "Admin")]
        public IActionResult Put(RoleDTO role)
        {
            var result = _roleService.CreateUpdateRole(role, Constants.Operation.Update);
            return Ok(result);
        }

        [HttpDelete("delete/{Id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(Guid Id)
        {
            var result = _roleService.DeleteRole(Id);
            return Ok(result);
        }


    }
}