/**
* @file IMemberController.cs
* @author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Declaration of controller for member
*/

using IIS_SERVER.Enums;
using IIS_SERVER.Member.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Member.Controllers;

public interface IMemberController
{
    Task<IActionResult> AddMember(MemberModel member);
    Task<IActionResult> ModeratorRequested(string handle, string email);
    Task<IActionResult> JoinRequested(string handle, string email);
    Task<IActionResult> DeleteJoinRequest(string handle, string email);
    Task<IActionResult> DeleteModeratorRequest(string handle, string email);
    Task<IActionResult> CreateJoinRequest(RequestDataModel model);
    Task<IActionResult> CreateModeratorRequest(RequestDataModel model);
    Task<IActionResult> DeleteMember(string email, string handle);
    Task<IActionResult> UpdateMemberRole(string email, GroupRole role, string handle);
    Task<IActionResult> GetMembers(string handle, GroupRole? role, int currentPage, int itemsPerPage);
    Task<IActionResult> GetMembersCount(string Handle);
    Task<IActionResult> UserInGroup(string email, string handle);
    Task<IActionResult> GetMemberRole(string email, string handle);
}
