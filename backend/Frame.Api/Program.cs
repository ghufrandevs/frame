using Frame.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database, security (and later: repositories, email, payment, auth)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ===== Startup: migrate the database and seed studios + admin =====
await app.Services.InitializeDatabaseAsync();

// ===== HTTP pipeline =====
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();