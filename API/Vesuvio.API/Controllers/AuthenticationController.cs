using Vesuvio.Domain.DTO;
using Vesuvio.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Threading.Tasks;


namespace Vesuvio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get()
        {
            var result = "1";
            return Ok(result);

        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> SignUp([FromForm]SignupDTO signupUser)
        {
            var result = await _authenticationService.SignUp(signupUser);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Route("Signin")]
        [HttpPost]
        public async Task<IActionResult> Signin(SigninDTO signinUser)
        {
            string result = await _authenticationService.Authentication(signinUser.UserName, signinUser.Password);

            if (result == Constants.ErrorCodes.InvalidCredentials)
            {
                return BadRequest(new JsonResult("Invalid credentials."));
            }
            else if (result == Constants.ErrorCodes.UnAuthorized)
            {
                return Unauthorized();
            }

            return Ok(result);
        }
    }
}
