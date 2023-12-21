using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskStore.Model;
using TaskStore.Services;

namespace TaskStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(AddUserRequest adUserRequest)
        {
            ApiResponse response = new();
            try
            {
                response = await _userService.Register(adUserRequest);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(AddUserRequest adUserRequest)
        {
            ApiResponse response = new();
            try
            {
                response = await _userService.Login(adUserRequest);
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.InnerException?.Message ?? ex.Message;
            }
            return Ok(response);
        }
    }
}
