using Scalar.AspNetCore;
using Test_API_2.Main;

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

var clients = new List<Clients>
{
    new Clients { Id = 1, Name = "Alice", Email = "alice@example.com" },
    new Clients { Id = 2, Name = "Bob", Email = "bob@example.com" },
    new Clients { Id = 3, Name = "Charlie", Email = "charlie@example.com" },
    new Clients { Id = 4, Name = "Diana", Email = "diana@example.com" }
};

var tickets = new List<Tickets>
{
    new Tickets { Id = 1, Id_Client = 1, Title = "Ticket 1", Status = "Open" },
    new Tickets { Id = 2, Id_Client = 2, Title = "Ticket 2", Status = "Closed" },
    new Tickets { Id = 3, Id_Client = 3, Title = "Ticket 3", Status = "In Progress" },
    new Tickets { Id = 4, Id_Client = 4, Title = "Ticket 4", Status = "Open" }
};

app.MapGet("/", () => "Hello World!");

//Affiche tous les clients(info complête)
app.MapGet("/clients", () => clients);

//Affiche toutes les infos d'un client par son ID
app.MapGet("/clients/{id}", (int id) =>
{
    var client = clients.FirstOrDefault(c => c.Id == id);
    if (client == null)
    {
        return Results.NotFound("Client non trouvé");
    }
    return Results.Ok(client);
});

//Ajoute un client
app.MapPost("/clients", (Clients NewClient) =>
{
    if (string.IsNullOrEmpty(NewClient.Name))
    {
        return Results.BadRequest("Le nom du client est obligatoire");
    }
    if (string.IsNullOrEmpty(NewClient.Email))
    {
        return Results.BadRequest("L'email du client est obligatoire");
    }

    NewClient.Id = clients.Count > 0 ? clients.Max(c => c.Id) + 1 : 1;

    clients.Add(NewClient);

    return Results.Created($"/clients/{NewClient.Id}", NewClient.Name);
});

//Modifie un client
app.MapPut("/clients/{id}", (int id, Clients EditClient) =>
{
    var client = clients.FirstOrDefault(c => c.Id == id);
    if (client == null)
    {
        return Results.NotFound("Client non trouvé");
    }

    client.Name = EditClient.Name;
    client.Email = EditClient.Email;

    return Results.Ok(client);
});

//Supprime un client
app.MapDelete("/clients/{id}", (int id) =>
{
    var client = clients.FirstOrDefault(c => c.Id == id);
    if (client == null)
    {
        return Results.NotFound("Client non touvé");
    }
    clients.Remove(client);
    return Results.Ok($"Client :{client.Name} Supprimé avec succès.");
});

app.Run();
