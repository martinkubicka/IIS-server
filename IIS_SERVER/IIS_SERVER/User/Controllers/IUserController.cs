using IIS_SERVER.User.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.User.Controllers;

public class UpdateUserRequest
{
    public UserDetailModel updatedUser { get; set; }
    public UserPrivacySettingsModel userPrivacy { get; set; }
}

public interface IUserController
{
    Task<IActionResult> AddUser(UserDetailModel user);
    Task<IActionResult> GetUsersList();
    Task<IActionResult> GetUserProfile(string handle);
    Task<IActionResult> GetUserRole(string handle);
    Task<IActionResult> UpdateUser(UpdateUserRequest updatedUser);
    Task<IActionResult> DeleteUser(string email);
    Task<IActionResult> GetUserPrivacySettings(string handle);
}