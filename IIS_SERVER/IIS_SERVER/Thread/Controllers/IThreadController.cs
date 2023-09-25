using IIS_SERVER.Thread.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Thread.Controllers
{
    public interface IThreadController
    {
        Task<IActionResult> CreateThread(ThreadModel thread);
        Task<IActionResult> GetThread(string threadId);
        Task<IActionResult> UpdateThread(string threadId, ThreadModel updatedThread);
        Task<IActionResult> DeleteThread(string threadId);
        Task<IActionResult> GetAllThreads();
    }
}
