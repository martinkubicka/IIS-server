using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> AddUser(UserDetailModel user);
    Task<List<UserListModel?>> GetUsersList(int limit = 0);
    Task<Tuple<UserListModel?, string?>> GetUserProfile(string handle);
    Task<Tuple<Role?, string>> GetUserRole(string email);
    Task<Tuple<string?, string>> GetUserHandle(string email);
    Task<Tuple<bool, string?>> UpdateUser(
        UserDetailModel updatedUser,
        UserPrivacySettingsModel userPrivacy
    );

    Task<UserDetailModel?> GetUserBasicInfo(string email);
    Task<Tuple<bool, string?>> UpdateUserWithoutPassword(
        UserDetailPasswordNotRequiredModel updatedUser,
        UserPrivacySettingsModel userPrivacy
    );
    Task<Tuple<bool, string?>> DeleteUser(string email);
    Task<Tuple<UserPrivacySettingsModel?, string?>> GetUserPrivacySettings(string handle);

    Task<List<UserListModel?>> SearchUsers(string searchTerm, int limit);
}
