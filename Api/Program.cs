using System.Security.Claims;
using System.Text;
using Application;
using Application.Validator.User;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = SessionTokenDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = SessionTokenDefaults.AuthenticationScheme;
        options.DefaultScheme = SessionTokenDefaults.AuthenticationScheme;
    })
    .AddScheme<AuthenticationSchemeOptions, SessionTokenAuthenticationHandler>(
        SessionTokenDefaults.AuthenticationScheme,
        options => { }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));
});
builder.Services.AddAuthorization();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
    
    c.AddSecurityDefinition("SessionToken",new OpenApiSecurityScheme
    {
        Description = "Session Token (Access Token) - از لاگین بگیرید",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "SessionToken"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference =  new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "SessionToken"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray()
            );

        return new BadRequestObjectResult(new
        {
            errors
        });
    };
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();