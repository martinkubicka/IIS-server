using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> AddUser(UserDetailModel user);
    Task<List<UserListModel>?> GetUsersList();
    Task<UserListModel?> GetUserProfile(string handle);
    Task<Role?> GetUserRole(string handle);
    Task<bool> UpdateUser(UserDetailModel updatedUser, UserPrivacySettingsModel userPrivacy);
    Task<Tuple<bool, string?>> DeleteUser(string email);
    Task<UserPrivacySettingsModel?> GetUserPrivacySettings(string handle);
}
