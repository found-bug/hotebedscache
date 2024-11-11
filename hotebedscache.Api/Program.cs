using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hotebedscache.Domain.Entities;
using Hotebedscache.Service.Interfaces;
using Hotebedscache.Service;
using System.Text;
using Hotebedscache.Api.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<HotebedContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));
#region CONFIGURE Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Peer Fusion Service", Version = "V1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement(){{
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                      },
                        new List<string>()
                      }
                 });

});
#endregion

#region CONFIGURE JWT
var appSettings = builder.Configuration.GetSection("AppSettings:Secret");
var key = Encoding.ASCII.GetBytes(appSettings.Value.ToString());

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    // add an instance of the patched manager to the options:
    options.CookieManager = new ChunkingCookieManager();
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
#endregion

builder.Services.AddScoped<IMasterServices, MasterService>();
builder.Services.AddScoped<IAccountService, AccountService>(); 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllCors", builder =>
    {
        builder 
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowedToAllowWildcardSubdomains()
        .SetIsOriginAllowed(delegate (string requestingOrigin)
        {
            return true;
        }).Build();
    });
});
builder.Services.AddHttpClient();
var app = builder.Build(); 
app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("AllowAllCors");
app.UseSwagger(c =>
{
    c.SerializeAsV2 = true;
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotebedscache API ");
    c.DefaultModelExpandDepth(0);
    c.DefaultModelsExpandDepth(-1);
});

app.UseAuthorization();

app.MapControllers();

app.Run();
JobScheduler.Start();
#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
#pragma warning restore ASP0014 // Suggest using top level route registrations
