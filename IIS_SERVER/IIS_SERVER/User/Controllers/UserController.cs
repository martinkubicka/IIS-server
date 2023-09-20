using IIS_SERVER.Services;
using IIS_SERVER.User.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.User.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase, IUserController
{
    private readonly IMySQLService MySqlService;
    
    public UserController(IMySQLService mySqlService)
    {
        MySqlService = mySqlService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddUser(UserDetailModel user)
    {
        bool result = await MySqlService.AddUser(user);
        if (result)
        {
            return Ok("User successfully added to DB.");
        }
        else
        {
            return StatusCode(500, "Database error: Failed to add the user to the database.");
        }
        
    }
}
