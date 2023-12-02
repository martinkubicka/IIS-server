/**
* @file IThreadController.cs
* author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Declaration of thread controller
*/


using IIS_SERVER.Thread.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Thread.Controllers
{
    public interface IThreadController
    {
        Task<IActionResult> CreateThread(ThreadModel thread);
        Task<IActionResult> GetThread(Guid threadId);
        Task<IActionResult> GetAllThreads(int limit = 0);
        Task<IActionResult> GetThreadsFromSpecificGroup(string Handle,  int currentPage, int itemsPerPage, string? filterName, string? filterFromDate, string? filterToDate);
        Task<IActionResult> UpdateThread(Guid threadId, ThreadModel updatedThread);
        Task<IActionResult> DeleteThread(Guid threadId);
        Task<IActionResult> GetThreadsCount(string Handle, string? filterName, string? filterFromDate, string? filterToDate);
        Task<IActionResult> GetAllThreadsUserIsIn(string Email);
        Task<IActionResult> SearchThreads(string searchTerm, int limit);
    }
}
