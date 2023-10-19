using System.Security.Claims;
using System.Threading.Channels;
using IIS_SERVER.Enums;
using IIS_SERVER.Group.Models;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Services;
using IIS_SERVER.Member.Models;
using IIS_SERVER.User.Models;
using Microsoft.AspNetCore.Authorization;

namespace IIS_SERVER.Member.Controllers;

[ApiController]
[Route("[controller]")]
public class MemberController : ControllerBase, IMemberController
{
    private readonly IMySQLService MySqlService;

    public MemberController(IMySQLService mySqlService)
    {
        MySqlService = mySqlService;
    }

    [HttpPost("add")]
    [Authorize(Policy = "AdminUserPolicy")]
    public async Task<IActionResult> AddMember(MemberModel member)
    {
        if (User.IsInRole("admin") || member.Email == User.FindFirst(ClaimTypes.Email).Value)
        {
            Tuple<bool, string?> result = await MySqlService.AddMember(member);
            if (result.Item1)
            {
                return StatusCode(201, "Member successfully added to DB.");
            }
            else
            {
                if (result.Item2.Contains("exists"))
                {
                    return StatusCode(409, "Error: Member already added in group.");
                }
                else if (result.Item2.Contains("Groups"))
                {
                    return StatusCode(404, "Error: Group not found.");
                }
                else if (result.Item2.Contains("Users"))
                {
                    return StatusCode(404, "Error: User not found.");
                }
                else
                {
                    return StatusCode(500, "Error: " + result.Item2);
                }
            }
        }
        else
        {
            return Forbid();
        }
    }

    [HttpDelete("delete")]
    [Authorize(Policy="AdminUserPolicy")]
    public async Task<IActionResult> DeleteMember(string email, string handle)
    {
        if (User.IsInRole("admin") ||
            await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle) == GroupRole.admin || User.FindFirst(ClaimTypes.Email).Value == email)
        {
            Tuple<bool, string?> result = await MySqlService.DeleteMember(email, handle);

            return result.Item1
                ? StatusCode(204, "Member successfully deleted.")
                : result.Item2.Contains("admin")
                    ? StatusCode(403, "Error: Member is admin of the group.")
                    : result.Item2.Contains("Member")
                        ? StatusCode(404, "Error: Group or member not found.")
                        : StatusCode(500, "Error: " + result.Item2);
        }
        else
        {
            return Forbid();
        }
    }

    [HttpPut("updateRole")]
    [Authorize(Policy="AdminUserPolicy")]
    public async Task<IActionResult> UpdateMemberRole(string email, GroupRole role, string handle)
    {
        if (User.IsInRole("admin") ||
            await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle) == GroupRole.admin)
        {
            Tuple<bool, string?> result = await MySqlService.UpdateMemberRole(email, role, handle);

            return result.Item1
                ? StatusCode(204, "Member successfully updated.")
                : result.Item2.Contains("Groups")
                    ? StatusCode(404, "Error: Group or member not found.")
                    : StatusCode(500, "Error: " + result.Item2);
        }
        else
        {
            return Forbid();
        }
    }

    [HttpGet("getMembers")]
    public async Task<IActionResult> GetMembers(string handle, GroupRole? role, int currentPage, int itemsPerPage)
    {
        GroupPrivacySettingsModel privacySettings = await MySqlService.GetGroupPolicy(handle);
        if (privacySettings.VisibilityGuest ||
            User.IsInRole("admin") ||
            await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle) == GroupRole.admin ||
            (await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle) != null && privacySettings.VisibilityMember)
           )
        {
            Tuple<List<MemberModel>?, string?> result =
                await MySqlService.GetMembers(handle, role, currentPage, itemsPerPage);

            if (result.Item1 != null)
            {
                return StatusCode(200, result.Item1);
            }

            return result.Item2.Contains("Groups")
                ? StatusCode(404, "Error: Group not found.")
                : StatusCode(500, "Error: " + result.Item2);
        }
        else
        {
            return Forbid();
        }
    }
    
    [HttpGet("getMemberRole")]
    [Authorize(Policy="AdminUserPolicy")]
    public async Task<IActionResult> GetMemberRole(string email, string handle)
    {
        if (User.IsInRole("admin") ||
            await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle) == GroupRole.admin ||
            User.FindFirst(ClaimTypes.Email).Value == email)
        {
            GroupRole? role = await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle);

            if (role != null)
            {
                return StatusCode(200, role);
            }

            return StatusCode(404, "Error: Group or member not found.");
        }
        else
        {
            return Forbid();
        }
    }
    
    [HttpGet("GetMembersCount")]
    public async Task<IActionResult> GetMembersCount(string Handle)
    {
        GroupPrivacySettingsModel privacySettings = await MySqlService.GetGroupPolicy(Handle);
        if (User.IsInRole("admin") ||
            await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, Handle) == GroupRole.admin ||
            privacySettings.VisibilityGuest ||
            (await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, Handle) != null && privacySettings.VisibilityMember)
           )
        {
            try
            {
                int? count = await MySqlService.GetMembersCount(Handle);
                if (count != null)
                {
                    return StatusCode(200, count);
                }
                else
                {
                    return StatusCode(404, "Error: Group not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        else
        {
            return Forbid();
        }
    }
    
    [HttpGet("UserInGroup")]
    [Authorize(Policy="AdminUserPolicy")]
    public async Task<IActionResult> UserInGroup(string email, string handle)
    {
        if (User.IsInRole("admin") ||
            await MySqlService.GetMemberRole(User.FindFirst(ClaimTypes.Email).Value, handle) == GroupRole.admin ||
            User.FindFirst(ClaimTypes.Email).Value == email)
        {
            try
            {
                bool? result = await MySqlService.UserInGroup(email, handle);
                if (result != null)
                {
                    return StatusCode(200, result);
                }
                else
                {
                    return StatusCode(404, "Error: Group not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        else
        {
            return Forbid();
        }
    }
}
