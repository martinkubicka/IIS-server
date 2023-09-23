using IIS_SERVER.Enums;
using IIS_SERVER.Member.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Member.Controllers;

public interface IMemberController
{
    Task<IActionResult> AddMember(MemberModel member);
    Task<IActionResult> DeleteMember(string email);
    Task<IActionResult> UpdateMemberRole(string email, GroupRole role);
}
