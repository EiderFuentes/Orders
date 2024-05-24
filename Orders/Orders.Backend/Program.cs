using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Implementations;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Para evitar la redundancia ciclica en la respuesta de los JSON
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Habilitando tokens en swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders Backend", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                      Example: 'Bearer 12345abcdef'<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
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


//Inyectamos la coneccion de la base de datos
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
builder.Services.AddTransient<SeedDb>();//Inyectamos nuestro alimentador
builder.Services.AddScoped<IFileStorage, FileStorage>(); // Inyectamos nuestro blob stora
builder.Services.AddScoped<IMailHelper, MailHelper>();
builder.Services.AddScoped<IOrdersHelper, OrdersHelper>();

//Inyectamos la unidad de trabajo generico
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));
//Inyectamos el repositorio generico
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();// Inyectamos el repositorio de cuidades
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();//Inyectamos el repositorio de paises
builder.Services.AddScoped<IStatesRepository, StatesRepository>();//Inyectamos el repositorio de estados
builder.Services.AddScoped<IUsersRepository, UsersRepository>();//Inyectamos el repositorio de Usuarios
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ITemporalOrdersRepository, TemporalOrdersRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();


builder.Services.AddScoped<ICategoriesUnitOfWork, CategoriesUnitOfWork>();
builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();//Inyectamos la unidad de trabajo de cuidades
builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();//Inyectamos la unidad de trabajo de paises
builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();//Inyectamos la unidad de trabajo de estados
builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();//Inyectamos la unidad de trabajo de usuarios
builder.Services.AddScoped<IProductsUnitOfWork, ProductsUnitOfWork>();
builder.Services.AddScoped<ITemporalOrdersUnitOfWork, TemporalOrdersUnitOfWork>();
builder.Services.AddScoped<IOrdersUnitOfWork, OrdersUnitOfWork>();

//Validaciones del usuarios
builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    x.SignIn.RequireConfirmedEmail = true;//EmailConfirmado

    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //TODO:
    x.Lockout.MaxFailedAccessAttempts = 3;
    x.Lockout.AllowedForNewUsers = true;

})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

//Metodo del token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
        ClockSkew = TimeSpan.Zero
    });

var app = builder.Build();
SeedData(app);

void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}

//para habilitar su consumo agregamos la Seguridad del API Para que funcione sin ningun problema
app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
