using IIS_SERVER.Enums;
using IIS_SERVER.Member.Models;
using IIS_SERVER.Thread.Models;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public interface IMySQLService
{
    Task<Tuple<bool, string?>> AddUser(UserDetailModel user);
    Task<List<UserListModel>?> GetUsersList();
    Task<UserListModel?> GetUserProfile(string handle);
    Task<Role?> GetUserRole(string handle);
    Task<bool> UpdateUser(UserDetailModel updatedUser, UserPrivacySettingsModel userPrivacy);
    Task<Tuple<bool, string?>> DeleteUser(string email);
    Task<UserPrivacySettingsModel?> GetUserPrivacySettings(string handle);
    Task<Tuple<bool, string?>> AddMember(MemberModel member);
    Task<Tuple<bool, string?>> DeleteMember(string email, string handle);
    Task<Tuple<bool, string?>> UpdateMemberRole(string email, GroupRole role, string handle);


    // Thread methods
    Task<bool> CreateThread(ThreadModel thread);
    Task<ThreadModel?> GetThread(string threadId);
    Task<bool> UpdateThread(string threadId, ThreadModel updatedThread);
    Task<bool> DeleteThread(string threadId);
    Task<List<ThreadModel>?> GetAllThreads();
}