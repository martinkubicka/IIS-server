/**
* @file IMySQLService.Thread.cs
* author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Declaration of service for thread
*/


using IIS_SERVER.Thread.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> CreateThread(ThreadModel thread);
    Task<List<ThreadModel>?> GetAllThreads(int limit = 0);
    Task<List<ThreadModel>?> GetThreadsFromSpecificGroup(string Handle, int currentPage, int itemsPerPage, string? filterName, string? filterFromDate, string? filterToDate);
    Task<int?> GetThreadsCount(string Handle, string? filterName, string? filterFromDate, string? filterToDate);
    Task<ThreadModel?> GetThread(Guid threadId);
    Task<bool> UpdateThread(Guid threadId, ThreadModel updatedThread);
    Task<Tuple<bool, string?>> DeleteThread(Guid threadId);
    Task<List<ThreadModel>?> GetAllThreadsUserIsIn(string Email);
    
    Task<List<ThreadModel?>> SearchThreads(string searchTerm, int limit);
}
