using Microsoft.EntityFrameworkCore;
using RefugeeSkillsPlatform.Core.Interfaces.Services;
using RefugeeSkillsPlatform.Core.Interfaces;
using RefugeeSkillsPlatform.Infrastructure.Persistence;
using RefugeeSkillsPlatform.Infrastructure.Repositories.Services;
using RefugeeSkillsPlatform.Infrastructure.Repositories;
using RefugeeSkillsPlatform.WebApi.Middlewares;
using RefugeeSkillsPlatform.WebApi.Common;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RefugeeSkillsPlatform.Core.DTOs;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.Configure<ZoomSettings>(
    builder.Configuration.GetSection("ZoomSettings"));

builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDeliveryMethodsService, DeliveryMethodsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
builder.Services.AddScoped<IZoomService, ZoomService>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var secretKey = builder.Configuration["SecretKey"];
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? string.Empty)),
        // ClockSkew = TimeSpan.Zero,

    };

});
builder.Services.AddDbContext<RefugeeSkillsDbContext>(options => { 
options.UseSqlServer(dbConnection);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        var response = new StandardRequestResponse<object>
        {
            Status = 404,
            Success = false,
            Message = "Resource not found",
            Data = null
        };

        await context.Response.WriteAsJsonAsync(response);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
