using IIS_SERVER.Services;
using IIS_SERVER.Group.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> AddGroup(GroupEmailModel group)
        {
        try
        {
            bool result = await MySqlService.AddGroup(group);
            if (result)
            {
                return StatusCode(201, "Group successfully added to DB.");
            }
            else
            {
                return StatusCode(500, "Error: Failed to add the Group to the database.");
            }
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("{handle}")]
    public async Task<IActionResult> GetGroup(string handle)
    {
        
        try
        {
            var group = await MySqlService.GetGroup(handle);
            if (group != null)
            {
                return Ok(group);
            }
            else
            {
                return NotFound("Group not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("user/{handle}")]
    public async Task<IActionResult> GetGroupsUserIsIn(string handle)
    {
        try
        {
            var groups = await MySqlService.GetGroupsUserIsIn(handle);
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        try
        {
            var groups = await MySqlService.GetGroups();
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("user/{userEmail}/{joined}")]
    public async Task<IActionResult> GetGroups(string userEmail, bool joined)
    {
        try
        {
            var groups = await MySqlService.GetGroups(userEmail, joined);
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpDelete("remove/{handle}")]
    public async Task<IActionResult> DeleteGroup(string handle)
    {
        try
        {
            bool result = await MySqlService.DeleteGroup(handle);
            if (result)
            {
                return Ok("Group successfully removed.");
            }
            else
            {
                return NotFound("Group not found or failed to remove.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateGroup(GroupListModel listModel)
    {
        try
        {
            bool result = await MySqlService.UpdateGroup(listModel);
            if (result)
            {
                return Ok("Group successfully updated.");
            }
            else
            {
                return NotFound("Group not found or failed to update.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPut("updatePolicy")]
    public async Task<IActionResult> UpdateGroupPolicy(
        GroupPrivacySettingsModel privacySettingsModel, string handle
    )
    {
        try
        {
            bool result = await MySqlService.UpdateGroupPolicy(privacySettingsModel, handle);
            if (result)
            {
                return Ok("Group policy successfully updated.");
            }
            else
            {
                return NotFound("Group policy update failed.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
    
    [HttpGet("policy/{handle}")]
    public async Task<IActionResult> GetGroupPolicy(string handle)
    {
        try
        {
            var group = await MySqlService.GetGroupPolicy(handle);
            if (group != null)
            {
                return Ok(group);
            }
            else
            {
                return NotFound("Group not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
}
