/**
* @file IMySQLService.Member.cs
* author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Declaration of service for member
*/


using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> AddMember(MemberModel member);
    Task<bool> ModeratorRequested(string handle, string email);
    Task<bool> JoinRequested(string handle, string email);
    Task<List<string>?> GetModeratorRequests(string handle);
    Task<bool> DeleteJoinRequest(string email, string handle);
    Task<bool> DeleteModeratorRequest(string email, string handle);
    Task<List<string>?> GetJoinRequests(string handle);
    Task<Tuple<bool, string?>> CreateJoinRequest(string handle, string email);
    Task<Tuple<bool, string?>> CreateModeratorRequest(string handle, string email);
    Task<Tuple<bool, string?>> DeleteMember(string email, string handle);
    Task<Tuple<bool, string?>> UpdateMemberRole(string email, GroupRole role, string handle);
    Task<Tuple<List<MemberModel>?, string?>> GetMembers(
        string handle,
        GroupRole? role,
        int currentPage,
        int itemsPerPage
    );
    Task<int?> GetMembersCount(string Handle);
    Task<bool?> UserInGroup(string email, string handle);
    Task<GroupRole?> GetMemberRole(string email, string handle);

    Task<bool> IsMember(string email, string groupHandle);
}
