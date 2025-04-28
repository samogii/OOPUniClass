
using Microsoft.EntityFrameworkCore;
using OOPClass1.Data;
using OOPClass1.DTO;
using OOPClass1.Models;

var builder = WebApplication.CreateBuilder(args);
#region Controller Builder Services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
#endregion 



builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("Users"));


var app = builder.Build();
#region Controller App Build
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
#endregion 


app.MapGet("/users", async (AppDbContext db) =>
    await db.Users.ToListAsync());


app.MapGet("/users/{id}", async (Guid id, AppDbContext db) =>
    await db.Users.FindAsync(id)
        is User User
            ? Results.Ok(User)
            : Results.NotFound());

app.MapPost("/new-user", async (UserDTO user, AppDbContext db) =>
{
    User newUser = new User
    {
        Id = Guid.NewGuid(),
        Name = user.Name,
        Username = user.Username
    };
    db.Users.Add(newUser);

    await db.SaveChangesAsync();

    return Results.Created($"/users/{newUser.Id}", newUser);
});

app.MapPut("update-user", async (User user, AppDbContext db) => {
    var exist = db.Users.Find(user.Id);
    if (exist == null) 
        return Results.NotFound();
    exist.Name = user.Name;
    exist.Username = user.Username;
    db.Update(exist);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapDelete("delete-user/{id}", async (Guid id, AppDbContext db) => {
    var exist = db.Users.Find(id);
    if (exist == null)
        return Results.NotFound();
    db.Users.Remove(exist);
    await db.SaveChangesAsync();
    return Results.Ok($"The {id} Successfully Deleted");
});

app.Run();
