using Asp.Versioning;
using formdata.Api.Security;
using formdata.DataAccess;
using formdata.DataAccess.Repositories;
using formdata.DataAccess.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:"))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSingleton<JwtTokenGenerator>();

builder.Services.AddOpenApi();
builder.Services.AddTransient<IFormDataRepository, FormDataRepository>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
    
        ClockSkew = TimeSpan.FromMinutes(1)   
    };

    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT FAILED: " + context.Exception.Message);

            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogWarning(context.Exception, "JWT authentication failed");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("===== JWT CHALLENGE =====");
            Console.WriteLine($"Has Authorization header: {context.Request.Headers.ContainsKey("Authorization")}");
            Console.WriteLine($"Authorization value: {context.Request.Headers["Authorization"]}");
            Console.WriteLine($"Error: {context.Error}");
            Console.WriteLine($"ErrorDescription: {context.ErrorDescription}");
            Console.WriteLine("=========================");
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(
                """{"error":"Unauthorized","message":"Valid JWT token is required"}""");
        }
    };
});

builder.Services.AddAuthorization();  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApiVersioning(options =>
{    
    options.AssumeDefaultVersionWhenUnspecified = true;

    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),                                      
        new HeaderApiVersionReader("X-Version"),                              
        new QueryStringApiVersionReader("api-version")                        
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();   
    app.UseSwaggerUI(); 
}

app.UseCors("AllowReactApp");

app.UseAuthentication();   
app.UseAuthorization();

app.MapControllers();

app.Run();
