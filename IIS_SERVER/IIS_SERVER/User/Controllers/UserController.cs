using IIS_SERVER.Services;
using IIS_SERVER.User.Models;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Enums;

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
        Tuple<bool, string?> result = await MySqlService.AddUser(user);
        if (result.Item1)
        {
            return StatusCode(201, "User successfully added to DB.");
        }
        else
        {
            if (result.Item2.Contains("PRIMARY"))
            {
                return StatusCode(409, "Error: The user with email already exists.");
            }
            else
            {
                return StatusCode(500, "Error: " + result.Item2);   
            }
        }
    }
    
    [HttpGet("getUsers")]
    public async Task<IActionResult> GetUsersList()
    {
        List<UserListModel>? usrs = await MySqlService.GetUsersList();

        if (usrs != null)
        {
            return StatusCode(200, usrs);
        }

        return StatusCode(500, "Error: DB error occured.");
    }
    
    [HttpGet("role")]
    public async Task<IActionResult> GetUserRole(string email)
    {
        Tuple<Role?, string?> result = await MySqlService.GetUserRole(email);

        if (result.Item1 != null)
        {
            return StatusCode(200, result.Item1);
        }
        
        return result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2);
    }
    
    [HttpGet("handle")]
    public async Task<IActionResult> GetUserHandle(string email)
    {
        Tuple<string?, string?> result = await MySqlService.GetUserHandle(email);

        if (result.Item1 != null)
        {
            return StatusCode(200, result.Item1);
        }
        
        return result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest updatedUser)
    {
        Tuple<bool, string?> result = await MySqlService.UpdateUser(updatedUser.updatedUser, updatedUser.userPrivacy);

        return result.Item1 ? StatusCode(204, "User successfully updated.") : result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2);
    }

    [HttpPut("updateWithoutPassword")]
    public async Task<IActionResult> UpdateUserWithoutPassword(UpdateUserRequestWithoutPassword updatedUser)
    {
        Tuple<bool, string?> result = await MySqlService.UpdateUserWithoutPassword(updatedUser.updatedUser, updatedUser.userPrivacy);

        return result.Item1 ? StatusCode(204, "User successfully updated.") : result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        Tuple<bool, string?> result = await MySqlService.DeleteUser(email);

        return result.Item1 ? StatusCode(204, "User successfully deleted.") : (result.Item2.Contains("admin") ? (result.Item2.Contains("group") ? StatusCode(403, "Error: User cannot be deleted because is an admin in one or more groups.") : StatusCode(403, "Error: User cannot be deleted because is an system admin.")) : result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2));
    }

    [HttpGet("privacy")]
    public async Task<IActionResult> GetUserPrivacySettings(string handle)
    {
        Tuple<UserPrivacySettingsModel?, string?> result = await MySqlService.GetUserPrivacySettings(handle);

        if (result.Item1 != null)
        {
            return StatusCode(200, result.Item1);
        }
        
        return result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile(string handle)
    {
        Tuple<UserListModel?, string?> result = await MySqlService.GetUserProfile(handle);

        if (result.Item1 != null)
        {
            return StatusCode(200, result.Item1);
        }

        return result.Item2.Contains("Users") ? StatusCode(404, "Error: User not found.") : StatusCode(500, "Error: " + result.Item2);
    }
}
