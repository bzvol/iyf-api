using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IPostRepository
{
    public IEnumerable<PostResponse> GetAllPosts();
    public Task<IEnumerable<PostAuthorizedResponse>> GetAllPostsAuthorized();
    public PostResponse GetPost(ulong id);
    public Task<PostAuthorizedResponse> GetPostAuthorized(ulong id);
    public Task<PostResponse> CreatePost(CreatePostRequest value, string userId);
    public Task<PostResponse> UpdatePost(ulong id, UpdatePostRequest value, string userId);
    public Task<PostResponse> DeletePost(ulong id);
}