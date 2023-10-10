using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> AddMember(MemberModel member);
    Task<Tuple<bool, string?>> DeleteMember(string email, string handle);
    Task<Tuple<bool, string?>> UpdateMemberRole(string email, GroupRole role, string handle);
    Task<Tuple<List<MemberModel>?, string?>> GetMembers(string handle, GroupRole? role, int currentPage, int itemsPerPage);
    Task<int?> GetMembersCount(string Handle);
}
