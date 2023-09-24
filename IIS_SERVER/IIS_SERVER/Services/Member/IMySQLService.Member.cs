using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> AddMember(MemberModel member);
    Task<Tuple<bool, string?>> DeleteMember(string email, string handle);
    Task<Tuple<bool, string?>> UpdateMemberRole(string email, GroupRole role, string handle);
}
