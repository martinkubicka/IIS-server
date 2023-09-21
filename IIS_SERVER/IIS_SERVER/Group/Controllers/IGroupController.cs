using IIS_SERVER.Group.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Group.Controllers;

public interface IGroupController
{
    Task<IActionResult> AddGroup(GroupDetailModel group);
}