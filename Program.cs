using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TestApiSalon.Data;
using TestApiSalon.Middlewares;
using TestApiSalon.Models;
using TestApiSalon.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connections = new Dictionary<DbConnectionName, string>
{
    { DbConnectionName.Client, "TestConnection"}
};
builder.Services.AddSingleton<IDictionary<DbConnectionName, string>>(connections);
builder.Services.AddSingleton<DataContext>();
builder.Services.AddScoped<IClaimsIdentityService<Customer>, CustomerClaimsIdentityService>();
builder.Services.AddScoped<ITokenService, JwtService>();
builder.Services.AddScoped<IHashService, SHA384HashService>();
builder.Services.AddScoped<IDbConnectionManager, DbConnectionManager>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<ErrorHandlerMiddleware>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.UseMiddleware<DbConnectionMiddleware>();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
