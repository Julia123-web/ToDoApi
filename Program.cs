using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseSqlite("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors(options =>

// this is necessary in order to connect to localhost endpoint. Shouldn't be used in production.
{
  options.AddPolicy(name: "AllowReact",
    policy =>
    {
      policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowReact");

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());


app.MapPost("/todoitems", async (Todo? todo, TodoDb db) =>
{
  if (todo == null)
  {
    return Results.BadRequest("It's mandatory to fill the field.");
  }

  //validation
  if (String.IsNullOrEmpty(todo.Name))
  {
    return Results.BadRequest("It's mandatory to fill the task.");
  }

  db.Todos.Add(todo);
  await db.SaveChangesAsync();

  return Results.Created();
});


app.Run();
