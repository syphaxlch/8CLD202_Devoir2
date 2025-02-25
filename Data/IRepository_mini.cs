using Microsoft.AspNetCore.Http.HttpResults;
using MVC.Models;

namespace MVC.Data
{
    public interface IRepository_mini
    {
        Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post);
        Task<Post?> GetPostById(Guid id);

        // Méthodes pour créer un commentaire
        Task<Results<Created<CommentReadDTO>, BadRequest, InternalServerError>> CreateAPICcomment(Comment comment);


        // Méthodes pour incrémenter les likes et dislikes des commentaires et posts
        Task<Results<Ok<Comment>, BadRequest, InternalServerError>> APlIncrementCommentLike(Guid commentId);
        Task<Results<Ok<Comment>, BadRequest, InternalServerError>> APlIncrementCommentDislike(Guid commentId);
        Task<Results<Ok<Post>, BadRequest, InternalServerError>> APlIncrementPostLike(Guid postId);
        Task<Results<Ok<Post>, BadRequest, InternalServerError>> APlIncrementPostDislike(Guid postId);

        // Méthodes pour récupérer des commentaires et posts
        Task<Results<Ok<CommentReadDTO>, NotFound, InternalServerError>> GetAPICcomment(Guid commentId);
    }

}
