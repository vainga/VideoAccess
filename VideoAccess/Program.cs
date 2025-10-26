using VideoAccess.Data;
using Microsoft.EntityFrameworkCore;
using VideoAccess.Services; // твой gRPC сервис
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- EF Core ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Razor Pages ---
builder.Services.AddRazorPages();

// --- gRPC ---
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// --- Swagger ---
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VideoAccess API",
        Version = "v1",
        Description = "gRPC + REST API для загрузки и управления видео"
    });
});

var app = builder.Build();

// --- Пайплайн ---
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VideoAccess API V1");
        c.RoutePrefix = string.Empty; // Swagger UI открывается по http://localhost:5000/
    });

    app.MapGrpcReflectionService();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// --- gRPC + Razor ---
app.MapGrpcService<VideoAccessService>();
app.MapRazorPages();

app.Run();
