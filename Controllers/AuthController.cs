using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Services;

namespace SciFiReviewsApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCredentials([FromBody] SignInModel signInModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.ValidateCredentials(signInModel))
                    return Ok(true);

                return Ok(false);
            }

            return BadRequest();
        }

        [HttpPut("create")]
        public async Task<IActionResult> CreateNewUser([FromBody] SignUpModel signUpModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.AddUser(signUpModel))
                    return Ok(true);

                return Ok(false);
            }

            return BadRequest();
        }
    }
}