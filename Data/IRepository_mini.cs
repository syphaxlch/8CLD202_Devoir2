using Microsoft.AspNetCore.Http.HttpResults;
using MVC.Models;

namespace MVC.Data
{
    public interface IRepository_mini
    {
        Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post);
        Task<Post?> GetPostById(Guid id);

    }

}
