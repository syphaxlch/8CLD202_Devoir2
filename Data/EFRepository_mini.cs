using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public class EFRepository_mini<TContext> : IRepository_mini where TContext : DbContext
    {
        protected readonly TContext _context;

        protected EFRepository_mini(TContext context)
        {
            this._context = context;
        }

        public virtual async Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post)
        {
            try
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return TypedResults.Created($"/Posts/{post.Id}", new PostReadDTO(post));
            }
            catch (Exception ex) when (ex is DbUpdateException)
            {
                return TypedResults.BadRequest();
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        }

        public async Task<Post?> GetPostById(Guid id)
        {
            return await _context.Set<Post>().FindAsync(id);
        }






        public async Task<Results<Created<CommentReadDTO>, BadRequest, InternalServerError>> CreateAPICcomment(Comment comment)
        {
            try
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();

                return TypedResults.Created($"/Comments/{comment.Id}", new CommentReadDTO(comment));
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        }




        public async Task<Results<Ok<Comment>, BadRequest, InternalServerError>> APlIncrementCommentLike(Guid commentId)
        {
            var comment = await _context.Set<Comment>().FindAsync(commentId);
            if (comment == null)
                return TypedResults.BadRequest();

            comment.IncrementLike();
            await _context.SaveChangesAsync();

            return TypedResults.Ok(comment);
        }

        public async Task<Results<Ok<Comment>, BadRequest, InternalServerError>> APlIncrementCommentDislike(Guid commentId)
        {
            var comment = await _context.Set<Comment>().FindAsync(commentId);
            if (comment == null)
                return TypedResults.BadRequest();

            comment.IncrementDislike();
            await _context.SaveChangesAsync();

            return TypedResults.Ok(comment);
        }

        public async Task<Results<Ok<Post>, BadRequest, InternalServerError>> APlIncrementPostLike(Guid postId)
        {
            var post = await _context.Set<Post>().FindAsync(postId);
            if (post == null)
                return TypedResults.BadRequest();

            post.IncrementLike();
            await _context.SaveChangesAsync();

            return TypedResults.Ok(post);
        }

        public async Task<Results<Ok<Post>, BadRequest, InternalServerError>> APlIncrementPostDislike(Guid postId)
        {
            var post = await _context.Set<Post>().FindAsync(postId);
            if (post == null)
                return TypedResults.BadRequest();

            post.IncrementDislike();
            await _context.SaveChangesAsync();

            return TypedResults.Ok(post);
        }

        public async Task<Results<Ok<CommentReadDTO>, NotFound, InternalServerError>> GetAPICcomment(Guid commentId)
        {
            var comment = await _context.Set<Comment>().FindAsync(commentId);
            if (comment == null)
                return TypedResults.NotFound();

            return TypedResults.Ok(new CommentReadDTO(comment));
        }



    }

}
