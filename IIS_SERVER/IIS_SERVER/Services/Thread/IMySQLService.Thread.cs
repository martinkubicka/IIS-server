using IIS_SERVER.Thread.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> CreateThread(ThreadModel thread);
    Task<List<ThreadModel>?> GetAllThreads();
    Task<ThreadModel?> GetThread(string threadId);
    Task<bool> UpdateThread(string threadId, ThreadModel updatedThread);
    Task<Tuple<bool, string?>> DeleteThread(string threadId);
}
