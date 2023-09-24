using IIS_SERVER.Enums;
using IIS_SERVER.Member.Models;
using IIS_SERVER.Thread.Models;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public interface IMySQLService
{
    // User
    Task<Tuple<bool, string?>> AddUser(UserDetailModel user);
    Task<List<UserListModel>?> GetUsersList();
    Task<Tuple<UserListModel?, string?>> GetUserProfile(string handle);
    Task<Tuple<Role?, string?>> GetUserRole(string handle);
    Task<Tuple<bool, string?>> UpdateUser(UserDetailModel updatedUser, UserPrivacySettingsModel userPrivacy);
    Task<Tuple<bool, string?>> DeleteUser(string email);
    Task<Tuple<UserPrivacySettingsModel?, string?>> GetUserPrivacySettings(string handle);
    
    // Member
    Task<Tuple<bool, string?>> AddMember(MemberModel member);
    Task<Tuple<bool, string?>> DeleteMember(string email, string handle);
    Task<Tuple<bool, string?>> UpdateMemberRole(string email, GroupRole role, string handle);


    // Thread methods
    Task<Tuple<bool, string?>> CreateThread(ThreadModel thread);
    Task<List<ThreadModel>?> GetAllThreads();
    Task<ThreadModel?> GetThread(string threadId);
    Task<bool> UpdateThread(string threadId, ThreadModel updatedThread);
    Task<Tuple<bool, string?>> DeleteThread(string threadId);
}