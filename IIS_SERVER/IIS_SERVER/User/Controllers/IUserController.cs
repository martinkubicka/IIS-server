using IIS_SERVER.User.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.User.Controllers;

public interface IUserController
{
    Task<IActionResult> AddUser(UserDetailModel user);
}