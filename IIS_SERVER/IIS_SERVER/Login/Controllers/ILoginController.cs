using IIS_SERVER.Login.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Login.Controllers;

public interface ILoginContoller
{
    Task<IActionResult> Login(LoginModel data);
    Task<IActionResult> Logout();
    Task<IActionResult> ForgotPassword(string email);
    Task<IActionResult> NewPassword(NewPasswordModel data);
}