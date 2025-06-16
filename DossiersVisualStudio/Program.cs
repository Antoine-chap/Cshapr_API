using API_csharp.Main;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
}

var produits = new List<Produit>
{
    new Produit { Id = 1, Nom = "Ordinateur portable", Prix = 999.99m, Description = "Super ordinateur pour le travail" },
    new Produit { Id = 2, Nom = "Souris", Prix = 25.50m, Description = "Souris sans fil très confortable" },
    new Produit { Id = 3, Nom = "Clavier", Prix = 75.00m, Description = "Clavier mécanique pour gamers" }
};

app.MapGet("/", () => "Bienvenue sur mon site !");

app.MapGet("/heure", () => $"il est actuellement : {DateTime.Now}");

app.MapGet("/apropos", () => "je suis développeur débutant en c#");

app.MapGet("/bonjour/{nom}", (string nom) => $"Bonjour {nom} !");

app.MapGet("/produits", () =>
{
    Console.WriteLine($"Quelqu'un a demandé la liste des produits. Il y en a {produits.Count}");
    return produits;
});

app.MapGet("/produits/{id}", (int id) =>
{
    var produit = produits.FirstOrDefault(p => p.Id == id);
    if (produit == null)
    {
        return Results.NotFound("Produit non trouvé");
    }
    return Results.Ok(produit);
});

app.MapPost("produits", (Produit nouveauProduit) =>
{
    if (string.IsNullOrEmpty(nouveauProduit.Nom))
    {
        return Results.BadRequest("Le nom du produit est obligatoire");
    }
    if (nouveauProduit.Prix <= 0)
    {
        return Results.BadRequest("Le prix doit être supérieur à 0");
    }
    nouveauProduit.Id = produits.Count > 0 ? produits.Max(p => p.Id) + 1 : 1;

    produits.Add(nouveauProduit);

    return Results.Created($"/produits/{nouveauProduit.Id}", nouveauProduit);
});

app.MapPut("/produits/{id}", (int id, Produit produitModifie) =>
{
    var produit = produits.FirstOrDefault(p => p.Id == id);
    if (produit == null)
    {
        return Results.NotFound("Produit non trouvé");
    }

    produit.Nom = produitModifie.Nom;
    produit.Prix = produitModifie.Prix;
    produit.Description = produitModifie.Description;

    return Results.Ok(produit);
});

app.MapDelete("/produits/{id}", (int id) =>
{
    var produit = produits.FirstOrDefault(p => p.Id == id);
    if (produit == null)
    {
        return Results.NotFound("Produit non trouvé");
    }

    produits.Remove(produit);

    return Results.Ok("Produit supprimé avec succès");
});


app.Run();
