using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> AddUser(UserDetailModel user);
    Task<List<UserListModel>?> GetUsersList();
    Task<Tuple<UserListModel?, string?>> GetUserProfile(string handle);
    Task<Tuple<Role?, string>> GetUserRole(string handle);
    Task<Tuple<bool, string?>> UpdateUser(UserDetailModel updatedUser, UserPrivacySettingsModel userPrivacy);
    Task<Tuple<bool, string?>> DeleteUser(string email);
    Task<Tuple<UserPrivacySettingsModel?, string?>> GetUserPrivacySettings(string handle);
}
