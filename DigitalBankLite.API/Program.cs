using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DigitalBank Lite API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 1. Database Context
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

// 2.1 Register Services
builder.Services.AddScoped<DigitalBankLite.API.Interfaces.IAuthService, DigitalBankLite.API.Services.AuthService>();
builder.Services.AddScoped<DigitalBankLite.API.Interfaces.IAccountService, DigitalBankLite.API.Services.AccountService>();
builder.Services.AddScoped<DigitalBankLite.API.Interfaces.ITransferService, DigitalBankLite.API.Services.TransferService>();
builder.Services.AddScoped<DigitalBankLite.API.Interfaces.IAdminService, DigitalBankLite.API.Services.AdminService>();
builder.Services.AddScoped<DigitalBankLite.API.Interfaces.IServiceRequestService, DigitalBankLite.API.Services.ServiceRequestService>();

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder.SetIsOriginAllowed(origin => true) // Allow any origin (VS vs VSCode ports)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<DigitalBankLite.API.Interfaces.IAuthService>();
    await authService.SeedAdminAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp"); // CORS MUST be before auth and redirection

app.UseMiddleware<DigitalBankLite.API.Middlewares.ExceptionMiddleware>();

// app.UseHttpsRedirection(); // Disabled to prevent CORS Preflight Redirect issue on Localhost

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
