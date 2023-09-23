using IIS_SERVER.Enums;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Services;
using IIS_SERVER.Member.Models;

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
    public async Task<IActionResult> AddMember(MemberModel member) // not found email, not found group, member already in group
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
            else
            {
                return StatusCode(404, "Error: User not found.");
            }
            
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteMember(string email)
    {
        bool result = await MySqlService.DeleteMember(email);

        return result
            ? StatusCode(204, "Member successfully deleted.")
            : StatusCode(404, "Error: Member not found.");
    }

    [HttpPut("updateRole")]
    public async Task<IActionResult> UpdateMemberRole(string email, GroupRole role)
    {
        bool result = await MySqlService.UpdateMemberRole(email, role);

        return result
            ? StatusCode(204, "Member successfully updated.")
            : StatusCode(404, "Error: Member not found.");
    }
}
