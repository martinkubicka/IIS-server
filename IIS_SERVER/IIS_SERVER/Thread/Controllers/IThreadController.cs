using IIS_SERVER.Thread.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Thread.Controllers
{
    public interface IThreadController
    {
        Task<IActionResult> CreateThread(ThreadModel thread);
        Task<IActionResult> GetThread(Guid threadId);
        Task<IActionResult> GetAllThreads();
        Task<IActionResult> GetThreadsFromSpecificGroup(string Handle,  int currentPage, int itemsPerPage, string? filterName, string? filterFromDate, string? filterToDate);
        Task<IActionResult> UpdateThread(Guid threadId, ThreadModel updatedThread);
        Task<IActionResult> DeleteThread(Guid threadId);
        Task<IActionResult> GetThreadsCount(string Handle, string? filterName, string? filterFromDate, string? filterToDate);
    }
}
