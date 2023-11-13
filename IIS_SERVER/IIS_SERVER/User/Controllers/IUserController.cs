using IIS_SERVER.User.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.User.Controllers;

public class UpdateUserRequest
{
    public UserDetailModel updatedUser { get; set; }
    public UserPrivacySettingsModel userPrivacy { get; set; }
}

public class UpdateUserRequestWithoutPassword
{
    public UserDetailPasswordNotRequiredModel updatedUser { get; set; }
    public UserPrivacySettingsModel userPrivacy { get; set; }
}

public interface IUserController
{
    Task<IActionResult> AddUser(UserDetailModel user);
    Task<IActionResult> GetUsersList(int limit = 0);
    Task<IActionResult> GetUserProfile(string handle);
    Task<IActionResult> GetUserRole(string email);
    Task<IActionResult> GetUserHandle(string email);
    Task<IActionResult> UpdateUser(UpdateUserRequest updatedUser);
    Task<IActionResult> UpdateUserWithoutPassword(UpdateUserRequestWithoutPassword updatedUser);
    Task<IActionResult> DeleteUser(string email);
    Task<IActionResult> GetUserPrivacySettings(string handle);

    Task<IActionResult> SearchUsers(string searchTerm, int limit);
}
