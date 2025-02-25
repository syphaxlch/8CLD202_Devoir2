//dotnet add package Microsoft.EntityFrameworkCore.InMemory
//dotnet add package Microsoft.EntityFrameworkCore

using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using MVC.Business;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ✅ Register BlobController in Dependency Injection (DI)
builder.Services.AddScoped<BlobController>();


// BD InMemory ...
builder.Services.AddDbContext<ApplicationDbContextInMemory>(options => options.UseInMemoryDatabase("InMemoryDb"));
builder.Services.AddScoped<IRepository_mini, EFRepository_mini_InMemory>();

// Ajouter le service pour Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.OperationFilter<FileUploadOperationFilter>() // Add custom operation filter
);
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection"]);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//API 
app.MapPost("/Posts/Add", async (IRepository_mini repo, IFormFile Image, HttpRequest request, HttpContext context, [FromServices] BlobController blob) =>
{
    try
    {
        PostCreateDTO post = new PostCreateDTO(request.Form["Title"]!, request.Form["Category"]!, request.Form["User"]!, Image);
        Guid guid = Guid.NewGuid();
        Console.WriteLine("Guid: " + guid);
        Console.WriteLine("Title: " + post.Title);
        Console.WriteLine("Guid: " + post.Category);


        string Url = await blob.PushImageToBlob(post.Image!, guid);

        var Post = new Post { Title = post.Title!, Category = post.Category, User = post.User!, BlobImage = guid, Url = Url };
        return await repo.CreateAPIPost(Post);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return TypedResults.BadRequest();
    }
}).DisableAntiforgery();


// Route pour obtenir un post spécifique
app.MapGet("/Posts/{id}", async Task<IResult>(Guid id, IRepository_mini repo) =>
{
    var post = await repo.GetPostById(id);
    if (post is null)
        return TypedResults.NotFound();

    return TypedResults.Ok(new PostReadDTO(post));
}).WithName("GetPostById");






// Route pour incrémenter le like d'un commentaire
app.MapPost("/Comments/{commentId}/Like", async Task<IResult> (Guid commentId, IRepository_mini repo) =>
{
    var result = await repo.APlIncrementCommentLike(commentId);
    return result;
});

// Route pour incrémenter le dislike d'un commentaire
app.MapPost("/Comments/{commentId}/Dislike", async Task<IResult> (Guid commentId, IRepository_mini repo) =>
{
    var result = await repo.APlIncrementCommentDislike(commentId);
    return result;
});

// Route pour incrémenter le like d'un post
app.MapPost("/Posts/{postId}/Like", async Task<IResult> (Guid postId, IRepository_mini repo) =>
{
    var result = await repo.APlIncrementPostLike(postId);
    return result;
});

// Route pour incrémenter le dislike d'un post
app.MapPost("/Posts/{postId}/Dislike", async Task<IResult> (Guid postId, IRepository_mini repo) =>
{
    var result = await repo.APlIncrementPostDislike(postId);
    return result;
});

// Route pour obtenir un commentaire spécifique
app.MapGet("/Comments/{commentId}", async Task<IResult> (Guid commentId, IRepository_mini repo) =>
{
    var result = await repo.GetAPICcomment(commentId);
    return result;
});



app.MapPost("/Comments/Add", async (IRepository_mini repo, [FromForm] CommentCreateDTO commentCreateDTO) =>
{
    try
    {
        // Créer un Comment à partir du DTO
        var comment = CommentCreateDTO.GetComment(commentCreateDTO);

        // Passer le Comment au repository pour création
        var result = await repo.CreateAPICcomment(comment);

        return result;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
        return TypedResults.BadRequest();
    }
}).DisableAntiforgery();


app.Run();


