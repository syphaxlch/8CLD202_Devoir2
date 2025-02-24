using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    // Classe spécialisée dans la gestion de la base de données en mémoire (ApplicationDbContextInMemory)
    public class EFRepository_mini_InMemory : EFRepository_mini<ApplicationDbContextInMemory>
    {
        // Le constructeur qui permet d'utiliser le contexte en mémoire (ApplicationDbContextInMemory)
        public EFRepository_mini_InMemory(ApplicationDbContextInMemory context) : base(context) { }

        // Création d'un nouveau post via l'API
        public override async Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post)
        {
            try
            {
                // Ajout du Post dans la base de données en mémoire
                await _context.Posts.AddAsync(post);

                // Sauvegarde des modifications dans la base de données en mémoire
                await _context.SaveChangesAsync();

                // Retour d'une réponse 201 (Created) avec un DTO contenant les informations du post créé
                return TypedResults.Created($"/Posts/{post.Id}", new PostReadDTO(post));
            }
            catch (Exception ex) when (ex is DbUpdateException)
            {
                // Si une exception liée à la mise à jour de la base de données se produit
                return TypedResults.BadRequest(); // Retourner une réponse BadRequest
            }
            catch (Exception)
            {
                // En cas d'autre exception, on retourne une réponse InternalServerError
                return TypedResults.InternalServerError();
            }
        }

        // Méthode pour obtenir la liste des posts avec pagination
        public async Task<List<Post>> GetPostsIndex(int pageNumber, int pageSize)
        {
            // Utilisation de la pagination : on saute les posts précédents et on prend uniquement un certain nombre de posts
            return await _context.Posts
                                 .Skip((pageNumber - 1) * pageSize)  // Sauter les posts des pages précédentes
                                 .Take(pageSize)  // Prendre un nombre limité de posts selon la taille de la page
                                 .ToListAsync();  // Exécution de la requête en base de données en mémoire
        }

        // Méthode pour obtenir le nombre total de posts (utile pour la pagination)
        public async Task<int> GetPostsCount()
        {
            // Récupérer le nombre total de posts dans la base en mémoire
            return await _context.Posts.CountAsync();
        }

        // Méthode pour incrémenter les "likes" d'un post
        public async Task IncrementPostLike(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);  // Recherche du post par son ID
            if (post != null)
            {
                post.IncrementLike();  // Appel de la méthode qui incrémente le nombre de likes
                await _context.SaveChangesAsync();  // Sauvegarde de l'état modifié
            }
        }

        // Méthode pour incrémenter les "dislikes" d'un post
        public async Task IncrementPostDislike(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);  // Recherche du post par son ID
            if (post != null)
            {
                post.IncrementDislike();  // Appel de la méthode qui incrémente le nombre de dislikes
                await _context.SaveChangesAsync();  // Sauvegarde de l'état modifié
            }
        }

        // Méthode pour obtenir les commentaires d'un post spécifique
        public async Task<List<Comment>> GetCommentsIndex(Guid postId)
        {
            // Recherche de tous les commentaires associés au post
            return await _context.Comments
                                 .Where(c => c.PostId == postId)  // On filtre les commentaires par l'ID du post
                                 .OrderBy(c => c.Created)  // On trie les commentaires par date de création
                                 .ToListAsync();  // Exécution de la requête en base de données en mémoire
        }

        // Méthode pour ajouter un commentaire à un post
        public async Task AddComment(Comment comment)
        {
            var post = await _context.Posts.FindAsync(comment.PostId);  // Recherche du post associé au commentaire
            if (post != null)
            {
                post.Comments.Add(comment);  // On ajoute le commentaire au post
                await _context.SaveChangesAsync();  // Sauvegarde des modifications dans la base en mémoire
            }
        }

        // Méthode pour incrémenter les "likes" d'un commentaire
        public async Task IncrementCommentLike(Guid commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);  // Recherche du commentaire par son ID
            if (comment != null)
            {
                comment.IncrementLike();  // Incrémentation des likes du commentaire
                await _context.SaveChangesAsync();  // Sauvegarde des modifications
            }
        }

        // Méthode pour incrémenter les "dislikes" d'un commentaire
        public async Task IncrementCommentDislike(Guid commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);  // Recherche du commentaire par son ID
            if (comment != null)
            {
                comment.IncrementDislike();  // Incrémentation des dislikes du commentaire
                await _context.SaveChangesAsync();  // Sauvegarde des modifications
            }
        }
    }
}
