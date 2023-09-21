using IIS_SERVER.Services;
using IIS_SERVER.Group.Models;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Enums;

namespace IIS_SERVER.Group.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController : ControllerBase, IGroupController
{
    private readonly IMySQLService MySqlService;

    public GroupController(IMySQLService mySqlService)
    {
        MySqlService = mySqlService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddGroup(GroupDetailModel Group)
    {
        bool result = await MySqlService.AddGroup(Group);
        if (result)
        {
            return StatusCode(201, "Group successfully added to DB.");
        }
        else
        {
            return StatusCode(500, "Error: Failed to add the Group to the database.");
        }
    }

    [HttpGet("getGroups")]
    public async Task<IActionResult> GetGroupsList()
    {
        List<GroupListModel>? usrs = await MySqlService.GetGroupsList();

        if (usrs != null)
        {
            return StatusCode(200, usrs);
        }

        return StatusCode(404, "Error: Group not found.");
    }

    [HttpGet("role")]
    public async Task<IActionResult> GetGroupRole(string handle)
    {
        Role? role = await MySqlService.GetGroupRole(handle);

        if (role != null)
        {
            return StatusCode(200, role);
        }

        return StatusCode(404, "Error: Group not found.");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateGroup(UpdateGroupRequest updatedGroup)
    {
        bool result = await MySqlService.UpdateGroup(
            updatedGroup.updatedGroup,
            updatedGroup.GroupPrivacy
        );

        return result
            ? StatusCode(204, "Group successfully updated.")
            : StatusCode(404, "Error: Group not found or DB error occured.");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteGroup(string email)
    {
        bool result = await MySqlService.DeleteGroup(email);

        return result
            ? StatusCode(204, "Group successfully deleted.")
            : StatusCode(404, "Error: Group not found or DB error occured.");
    }

    [HttpGet("privacy")]
    public async Task<IActionResult> GetGroupPrivacySettings(string handle)
    {
        GroupPrivacySettingsModel? privacySettings = await MySqlService.GetGroupPrivacySettings(
            handle
        );

        if (privacySettings != null)
        {
            return StatusCode(200, privacySettings);
        }

        return StatusCode(404, "Error: Group not found.");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetGroupProfile(string handle)
    {
        GroupListModel? profile = await MySqlService.GetGroupProfile(handle);

        if (profile != null)
        {
            return StatusCode(200, profile);
        }

        return StatusCode(404, "Error: Group not found.");
    }
}
