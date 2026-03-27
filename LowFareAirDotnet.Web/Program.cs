using LowFareAirDotnet.Infrastructure;
using LowFareAirDotnet.Logic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LowFareAirDotnet.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "LowFareAirDotnet API", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Wpisz: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
        });

        var keyJwt = Encoding.UTF8.GetBytes("lEEph41L2Tb8decvK41nAxPD6eWy9umzrVkqIN3cY3Q");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyJwt)
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (string.IsNullOrWhiteSpace(context.Token)
                        && context.Request.Cookies.TryGetValue("auth_token", out var cookieToken)
                        && !string.IsNullOrWhiteSpace(cookieToken))
                    {
                        context.Token = cookieToken;
                    }

                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var isApiRequest = context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
                    if (!isApiRequest)
                    {
                        context.HandleResponse();
                        var returnUrl = context.Request.Path + context.Request.QueryString;
                        context.Response.Redirect($"/Home/Login?returnUrl={Uri.EscapeDataString(returnUrl)}");
                    }

                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyFrontendApp", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });


        builder.Services.RegisterLogic();
        builder.Services.RegisterInfrastructure(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
