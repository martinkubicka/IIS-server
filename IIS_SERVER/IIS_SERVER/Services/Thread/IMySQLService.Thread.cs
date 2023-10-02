using IIS_SERVER.Thread.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> CreateThread(ThreadModel thread);
    Task<List<ThreadModel>?> GetAllThreads();
    Task<List<ThreadModel>?> GetThreadsFromSpecificGroup(string Handle, int currentPage, int itemsPerPage);
    Task<int?> GetThreadsCount(string Handle);
    Task<ThreadModel?> GetThread(Guid threadId);
    Task<bool> UpdateThread(Guid threadId, ThreadModel updatedThread);
    Task<Tuple<bool, string?>> DeleteThread(Guid threadId);
}
