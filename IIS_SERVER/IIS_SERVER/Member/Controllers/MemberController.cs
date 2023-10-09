using IIS_SERVER.Enums;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Services;
using IIS_SERVER.Member.Models;
using IIS_SERVER.User.Models;

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
    public async Task<IActionResult> AddMember(MemberModel member)
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

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteMember(string email, string handle)
    {
        Tuple<bool, string?> result = await MySqlService.DeleteMember(email, handle);

        return result.Item1
            ? StatusCode(204, "Member successfully deleted.")
            : result.Item2.Contains("admin") ? StatusCode(403, "Error: Member is admin of the group.") : result.Item2.Contains("Member") ? StatusCode(404, "Error: Group or member not found.") : StatusCode(500, "Error: " + result.Item2);
    }

    [HttpPut("updateRole")]
    public async Task<IActionResult> UpdateMemberRole(string email, GroupRole role, string handle)
    {
        Tuple<bool, string?> result = await MySqlService.UpdateMemberRole(email, role, handle);

        return result.Item1
            ? StatusCode(204, "Member successfully updated.")
            : result.Item2.Contains("Groups") ? StatusCode(404, "Error: Group or member not found.") : StatusCode(500, "Error: " + result.Item2);;
    }

    [HttpGet("getMembers")]
    public async Task<IActionResult> GetMembers(string handle, GroupRole? role, int currentPage, int itemsPerPage)
    {
        Tuple<List<UserListModel>?, string?> result = await MySqlService.GetMembers(handle, role, currentPage, itemsPerPage);

        if (result.Item1 != null)
        {
            return StatusCode(200, result.Item1);
        }
        
        return result.Item2.Contains("Groups") ? StatusCode(404, "Error: Group not found.") : StatusCode(500, "Error: " + result.Item2);
    }
    
    [HttpGet("GetMembersCount")]
    public async Task<IActionResult> GetMembersCount(string Handle)
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
}
