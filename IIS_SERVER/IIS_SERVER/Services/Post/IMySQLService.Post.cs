using IIS_SERVER.Post.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{

    Task<PostModel?> GetPost(Guid postId);

    Task<List<PostModel?>> GetPostsByThread(Guid threadId);

    Task<List<PostModel?>> GetPostsByUser(string userEmail);

    Task<Tuple<bool, string?>> AddPost(PostModel post);

    Task<Tuple<bool, string?>> EditPostText(Guid postId, string text);

    Task<Tuple<bool, string?>> EditPost(PostModel post);

    Task<Tuple<bool, string?>> DeletePost(PostModel post);
}
