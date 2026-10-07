using Frame.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database (and later: repositories, email, payment, auth)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ===== HTTP pipeline =====
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();