/**
* @file IGroupController.cs
* @author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Declaration of group controller
*/

using IIS_SERVER.Group.Models;
using IIS_SERVER.Member.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Group.Controllers;

public interface IGroupController
{
    Task<IActionResult> AddGroup(GroupMemberCompositeModel model);

    Task<IActionResult> GetGroup(string handle);

    Task<IActionResult> GetGroups(int limit = 0);
    Task<IActionResult> GetGroupsUserIsIn(string handle);

    //returns only groups that the user either joined or not joined depending on joined param
    Task<IActionResult> GetGroups(string userEmail, bool joined);

    Task<IActionResult> DeleteGroup(string handle);

    Task<IActionResult> UpdateGroup(GroupListModel listModel);

    Task<IActionResult> UpdateGroupPolicy(
        GroupPrivacySettingsModel privacySettingsModel,
        string handle
    );

    Task<IActionResult> GetGroupPolicy(string handle);

    Task<IActionResult> SearchGroups(string searchTerm, int limit);
}
