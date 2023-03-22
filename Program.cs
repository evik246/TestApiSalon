using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using TestApiSalon.Data;
using TestApiSalon.Middlewares;
using TestApiSalon.Models;
using TestApiSalon.Services.CategoryService;
using TestApiSalon.Services.ClaimsIdentityService;
using TestApiSalon.Services.ConnectionService;
using TestApiSalon.Services.CustomerService;
using TestApiSalon.Services.EmployeeService;
using TestApiSalon.Services.HashService;
using TestApiSalon.Services.ServiceService;
using TestApiSalon.Services.TokenService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connections = new Dictionary<DbConnectionName, string>
{
    { DbConnectionName.Guest, "TestConnection" },
    { DbConnectionName.Client, "TestConnection" }
};
builder.Services.AddSingleton<IDictionary<DbConnectionName, string>>(connections);
builder.Services.AddSingleton<DataContext>();
builder.Services.AddScoped<IClaimsIdentityService<Customer>, CustomerClaimsIdentityService>();
builder.Services.AddScoped<ITokenService, JwtService>();
builder.Services.AddScoped<IHashService, SHA384HashService>();
builder.Services.AddScoped<IDbConnectionService, DbConnectionService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "¬ведите токен в следующем формате: Bearer {ваш токен}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "StaticFiles")),
    RequestPath = ""
});

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.UseMiddleware<DbConnectionMiddleware>();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
