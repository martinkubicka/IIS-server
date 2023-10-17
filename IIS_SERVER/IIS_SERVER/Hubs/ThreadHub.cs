using Microsoft.AspNetCore.SignalR;

namespace IIS_SERVER.Hubs
{
    public class ThreadHub : Hub
    {
        public async Task JoinRoom(string threadId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, threadId);
        }

        public async Task LeaveRoom(string threadId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, threadId);
        }

        public async Task NewPost(string threadId, Guid postId)
        {
            try
            {
                await Clients.All.SendAsync("NewPost", postId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task DeletePost(string threadId, Guid postId)
        {
            try
            {
                await Clients.All.SendAsync("DeletePost", postId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
