using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vesuvio.Domain.DTO;
using Vesuvio.Service;
using Vesuvio.WebAPI.Helpers;

namespace Vesuvio.API.Controllers
{
    public class UserController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        private IWebHostEnvironment _webHostEnvironment;
        public string _baseUrl;

        public UserController(IUserService userService, IAuthenticationService authenticationService, IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _webHostEnvironment = webHostEnvironment;
            if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
            {
                _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
            _baseUrl = _webHostEnvironment.WebRootPath;
        }

        [HttpPost]
        [Route("getall")]
        [Authorize(Policy = "Admin")]
        public IActionResult Get(DataTableRequest request)
        {
            IEnumerable<UserDTO> usersData = _userService.GetAllUsers();

            if (usersData.Count() > 0)
            {
                var filteredUsersData = _userService.SearchUser(request, usersData.ToList());
                var pagedUsersData = _userService.PaginationUser(request, filteredUsersData);
                DataTableResponse jsonRes = new DataTableResponse
                {
                    RecordsTotal = usersData.ToList().Count,
                    Data = pagedUsersData.ToArray()
                };
                return Ok(jsonRes);
            }
            else return NotFound();
        }

        [HttpPost]
        [Route("usersbyrole/{roleId}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Get([FromBody] DataTableRequest request, Guid roleId)
        {
            var usersData = _userService.GetUsersByRole(roleId).ToList();
            if (usersData.Count() > 0)
            {
                var filteredUsersData = _userService.SearchUser(request, usersData);
                var pagedUsersData = _userService.PaginationUser(request, filteredUsersData);
                string jsonRes = JsonConvert.SerializeObject(new DataTableResponse
                {
                    RecordsTotal = usersData.Count(),
                    Data = pagedUsersData.ToArray()
                });
                return Ok(jsonRes);
            }
            else return NotFound();
        }

        [HttpGet]
        [Route("getbyid/{Id}")]
        [Authorize(Policy = "Customer")]
        public IActionResult GetById(Guid Id)
        {
            UserDTO usersData = _userService.GetUserById(Id);
            if (usersData != null)
            {
                return Ok(usersData);
            }
            else return NotFound();
        }


        [HttpGet]
        [Route("getcustomerorders/{userId}")]
        [Authorize(Policy = "Customer")]
        public IActionResult GetCustomerOrders(Guid userId)
        {
            CustomerOrders customerOrders = _userService.GetCustomerOrders(userId);

            if (customerOrders != null)
            {
                return Ok(customerOrders);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Post([FromForm] UserDTO userDTO)
        {
            var file = Request.Form.Files;
            var fileName = string.Empty;
            if (file.Count > 0)
            {
                string path = Path.Combine(_baseUrl, "Images/User");
                fileName = CommonHelper.SaveImage(path, file[0]);
            }

            userDTO.ImageUrl = fileName;
            var result = await _userService.CreateUser(userDTO);
            if (result.Succeeded) return Ok();
            else return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> Put([FromForm] UserDTO userDTO)
        {
            bool verifyCustomerUser = this._userService.VerifyCustomer(userDTO);
            if (verifyCustomerUser)
            {
                var file = Request.Form.Files;
                var fileName = string.Empty;

                string path = Path.Combine(_baseUrl, "Images/User");
                if (userDTO.RemoveImage)
                {
                    if (!string.IsNullOrEmpty(userDTO.ImageUrl))
                    {
                        CommonHelper.RemoveFile(path, userDTO.ImageUrl);
                        userDTO.ImageUrl = null;
                    }
                }

                if (file.Count > 0)
                {
                    fileName = CommonHelper.SaveImage(path, file[0]);
                    userDTO.ImageUrl = fileName;
                }

                var result = await _userService.UpdateUser(userDTO);

                if (result.Succeeded)
                {
                    return Ok();
                }
                else return BadRequest(result.Errors);
            }

            return BadRequest();
        }

        [HttpPatch]
        [Route("delete")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(List<Guid> ids)
        {
            var result = _userService.DeleteUser(ids);
            if (result) return Ok();
            else return BadRequest();
        }

        [HttpGet]
        [Route("currentuserid")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetCurrentUserId()
        {
            var user = _userService.GetCurrentUserId();
            return Ok(user);
        }

        [HttpPatch]
        [Route("changepassword")]
        [Authorize(Policy = "Customer")]
        public async Task<IActionResult> ChangeCustomerPassword([FromForm] PasswordChangeDTO passwordChangeDTO)
        {
            string result = await _userService.ChangePassword(passwordChangeDTO);

            if (result == "success")
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}