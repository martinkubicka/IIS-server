/**
* @file PostHelpers.cs
* author { Dominik Petrik (xpetri25) }
* @date 17.12.2023
* @brief Definition of helpers for post endpoints
*/

using IIS_SERVER.Enums;
using IIS_SERVER.Group.Models;
using IIS_SERVER.Post.Models;
using IIS_SERVER.Services;
using IIS_SERVER.Thread.Models;

namespace IIS_SERVER.Helpers
{
    public class PostHelper
    {
        public static async Task<GroupListModel> GetGroupByPostOrThread(
            Guid threadOrPostId,
            IMySQLService MySqlService
        )
        {
            ThreadModel? thread = await MySqlService.GetThread(threadOrPostId);

            if (thread == null)
            {
                PostModel? post =
                    await MySqlService.GetPost(threadOrPostId)
                    ?? throw new Exception("Could not find post");

                thread =
                    await MySqlService.GetThread(post.ThreadId)
                    ?? throw new Exception("Could not find thread");
            }
            return await MySqlService.GetGroup(thread.Handle)
                ?? throw new Exception("Could not find group");
            ;
        }

        public static async Task<bool> IsPosterInGroup(
            Guid threadOrPostId,
            string? email,
            IMySQLService MySqlService
        )
        {
            if (email == null)
            {
                throw new Exception("No email provided");
            }

            GroupListModel group = await GetGroupByPostOrThread(threadOrPostId, MySqlService);

            bool? isMember =
                await MySqlService.UserInGroup(email, group.Handle)
                ?? throw new Exception("Could not find group");
            return (bool)isMember;
        }

        public static async Task<bool> IsUserPostOwner(
            Guid postId,
            string? email,
            IMySQLService MySqlService
        )
        {
            if (email == null)
            {
                throw new Exception("No email provided");
            }
            PostModel? post =
                await MySqlService.GetPost(postId) ?? throw new Exception("Could not find post");

            var result = await MySqlService.GetUserHandle(email);
            if (result.Item1 == null)
            {
                throw new Exception("Could not find user");
            }

            return result.Item1 == post.Handle;
        }

        public static async Task<bool> IsUserPostOwner(
            string handle,
            string? email,
            IMySQLService MySqlService
        )
        {
            if (email == null)
            {
                throw new Exception("No email provided");
            }

            var result = await MySqlService.GetUserHandle(email);
            if (result.Item1 == null)
            {
                throw new Exception("Could not find user");
            }

            return result.Item1 == handle;
        }

        public static async Task<GroupRole> getMemberGroupRole(
            Guid postId,
            string email,
            IMySQLService MySqlService
        )
        {
            if (email == null)
            {
                throw new Exception("No email provided");
            }

            ThreadModel thread;
            PostModel? post =
                await MySqlService.GetPost(postId) ?? throw new Exception("Could not find post");

            thread =
                await MySqlService.GetThread(post.ThreadId)
                ?? throw new Exception("Could not find thread");

            return await MySqlService.GetMemberRole(email, thread.Handle)
                ?? throw new Exception("Could not find group or member");
        }

        public static async Task<bool> IsUserMemberByPost(
            Guid postId,
            string userEmail,
            IMySQLService MySqlService
        )
        {
            GroupListModel group =
                await GetGroupByPostOrThread(postId, MySqlService)
                ?? throw new Exception("Could not find group or member");
            Console.WriteLine(await MySqlService.IsMember(userEmail, group.Handle));
            return await MySqlService.IsMember(userEmail, group.Handle);
        }
    }
}
