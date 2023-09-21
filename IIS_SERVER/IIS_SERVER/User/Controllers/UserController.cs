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
        bool result = await MySqlService.AddUser(user);
        if (result)
        {
            return StatusCode(201, "User successfully added to DB.");
        }
        else
        {
            return StatusCode(500, "Error: Failed to add the user to the database.");
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

        return StatusCode(404, "Error: User not found.");
    }

    [HttpGet("role")]
    public async Task<IActionResult> GetUserRole(string handle)
    {
        Role? role = await MySqlService.GetUserRole(handle);

        if (role != null)
        {
            return StatusCode(200, role);
        }
        
        return StatusCode(404, "Error: User not found.");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest updatedUser)
    {
        bool result = await MySqlService.UpdateUser(updatedUser.updatedUser, updatedUser.userPrivacy);

        return result ? StatusCode(204, "User successfully updated.") : StatusCode(404, "Error: User not found or DB error occured.");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        bool result = await MySqlService.DeleteUser(email);

        return result ? StatusCode(204, "User successfully deleted.") : StatusCode(404, "Error: User not found or DB error occured.");
    }

    [HttpGet("privacy")]
    public async Task<IActionResult> GetUserPrivacySettings(string handle)
    {
        UserPrivacySettingsModel? privacySettings = await MySqlService.GetUserPrivacySettings(handle);

        if (privacySettings != null)
        {
            return StatusCode(200, privacySettings);
        }
        
        return StatusCode(404, "Error: User not found.");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile(string handle)
    {
        UserProfileModel? profile = await MySqlService.GetUserProfile(handle);

        if (profile != null)
        {
            return StatusCode(200, profile);
        }
        
        return StatusCode(404, "Error: User not found.");
    }
}
