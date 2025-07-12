using OrderService.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application layer services
builder.Services.AddApplication();

// Add health checks
builder.Services.AddHealthChecks();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Add root endpoint
app.MapGet("/", () => new
{
    Service = "OrderService.API",
    Version = "1.0.0",
    Status = "Running",
    Timestamp = DateTime.UtcNow,
    Endpoints = new
    {
        Health = "/health",
        ApiHealth = "/api/health", 
        Swagger = "/swagger",
        Orders = "/api/orders",
        SampleOrder = "/api/orders/sample"
    }
});

app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

app.Run();