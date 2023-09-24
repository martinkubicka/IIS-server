using IIS_SERVER.Group.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Group.Controllers;

public interface AddGroupRequest
{
    public GroupListModel Group { get; set; }
    public string OwnerEmail { get; set; }
}

public interface AddMemberRequest
{
    public string MemberEmail { get; set; }
    public string GroupHandle { get; set; }
}

public interface IGroupController
{
    Task<IActionResult> AddGroup(AddGroupRequest group);

    Task<IActionResult> GetGroup(string handle);

    Task<IActionResult> GetGroups();

    //returns only groups that the user either joined or not joined depending on joined param
    Task<IActionResult> GetGroups(string userEmail, bool joined);

    Task<IActionResult> DeleteGroup(string handle);

    Task<IActionResult> UpdateGroup(GroupListModel listModel);

    Task<IActionResult> UpdateGroupPolicy(GroupPrivacySettingsModel privacySettingsModel);
}
