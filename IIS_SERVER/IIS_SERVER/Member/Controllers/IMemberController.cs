using IIS_SERVER.Enums;
using IIS_SERVER.Member.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Member.Controllers;

public interface IMemberController
{
    Task<IActionResult> AddMember(MemberModel member);
    Task<IActionResult> DeleteMember(string email, string handle);
    Task<IActionResult> UpdateMemberRole(string email, GroupRole role, string handle);
    Task<IActionResult> GetMembers(string handle, GroupRole? role, int currentPage, int itemsPerPage);
    Task<IActionResult> GetMembersCount(string Handle);
    Task<IActionResult> UserInGroup(string email, string handle);
    Task<IActionResult> GetMemberRole(string email, string handle);
}
