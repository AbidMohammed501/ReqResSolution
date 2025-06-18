using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReqResApiIntegration.Models;
using ReqResApiIntegration.Services;

namespace ReqResApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IExternalUserService _externalUserService;

        public UsersController(IExternalUserService externalUserService)
        {
            _externalUserService = externalUserService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _externalUserService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, $"Service unavailable: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("id")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _externalUserService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                return Ok(user);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, $"Service unavailable: {ex.Message}");
            }
        }
    }
}
