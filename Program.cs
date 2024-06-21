using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using aula_api.db;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbLivrariaContext>(opt => {
    string connectionString = builder.Configuration.GetConnectionString("livrariaConnection");
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    opt.UseMySql(connectionString, serverVersion);
});

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/livros", ([FromServices] DbLivrariaContext _db, [FromQuery] string? nomeLivro) =>
{
    var query = _db.TbLivro.AsQueryable<TbLivro>(); 

    if (!String.IsNullOrEmpty(nomeLivro)) {
        query = query.Where(livro => livro.Titulo.Contains(nomeLivro));
    }

    var livroBuscando = query.ToList<TbLivro>();

    return Results.Ok(livroBuscando);
});

app.MapGet("/api/livros/{id}", ([FromServices] DbLivrariaContext _db, [FromRoute] int id) =>{
    var livro = _db.TbLivro.Find(id);

    if (livro == null){
        return Results.NotFound();
    }

    return Results.Ok(livro);
});

app.MapPost("/api/livros", ([FromServices] DbLivrariaContext _db, [FromBody] TbLivro novoLivro) => {
    if (String.IsNullOrEmpty(novoLivro.Titulo)){
        return Results.BadRequest(new { mensagem = "Não é possível incluir um livro sem título."});
    }

    var livro = new TbLivro
    {
        Titulo = novoLivro.Titulo,
        Ano = novoLivro.Ano,
        DsLivro = novoLivro.DsLivro,
        FkIdautor = novoLivro.FkIdautor,
        FkIdcategoria = novoLivro.FkIdcategoria
    };

    _db.TbLivro.Add(livro);
    _db.SaveChanges();

    var livroUrl = $"/api/livros/{livro.IdLivro}";

    return Results.Created(livroUrl, livro);
    });

    app.MapPut("/api/livros/{id}", ([FromServices] DbLivrariaContext _db, [FromRoute] int id, [FromBody] TbLivro livroEditado) =>
    {
        if(livroEditado.IdLivro != id){
            return Results.BadRequest(new { mensagem = "Id inexistente."});
        }

        if(String.IsNullOrEmpty(livroEditado.Titulo)){
            return Results.BadRequest(new {mensagem = "Não é possível alterar um livro sem Título."});
        }

        var Livro = _db.TbLivro.Find(id);

        if (Livro == null){
            return Results.NotFound();
        }

        Livro.Titulo = livroEditado.Titulo;
        Livro.DsLivro = livroEditado.DsLivro;
        Livro.Ano = livroEditado.Ano;
        Livro.FkIdautor = livroEditado.FkIdautor;
        Livro.FkIdcategoria = livroEditado.FkIdcategoria;

        _db.SaveChanges();

        return Results.Ok(Livro);
    });

    app.MapDelete("/api/livros/{id}", ([FromServices] DbLivrariaContext _db, [FromRoute] int id) =>
    {
        var livro = _db.TbLivro.Find(id);

        if (livro == null){
            return Results.NotFound();
        }

        _db.TbLivro.Remove(livro);
        _db.SaveChanges();

        return Results.Ok();
    });

app.Run();
