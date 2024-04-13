using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Implementations;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Para evitar la redundancia ciclica en la respuesta de los JSON
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inyectamos la coneccion de la base de datos
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
//Inyectamos nuestro alimentador
builder.Services.AddTransient<SeedDb>();

//Inyectamos la unidad de trabajo generico
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));
//Inyectamos el repositorio generico
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();// Inyectamos el repositorio de cuidades
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();//Inyectamos el repositorio de paises
builder.Services.AddScoped<IStatesRepository, StatesRepository>();//Inyectamos el repositorio de estados
builder.Services.AddScoped<IUsersRepository, UsersRepository>();//Inyectamos el repositorio de Usuarios

builder.Services.AddScoped<ICategoriesUnitOfWork, CategoriesUnitOfWork>();
builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();//Inyectamos la unidad de trabajo de cuidades
builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();//Inyectamos la unidad de trabajo de paises
builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();//Inyectamos la unidad de trabajo de estados
builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();//Inyectamos la unidad de trabajo de usuarios

//Validaciones del usuarios
builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();


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

//Seguridad del API Para que funcione sin ningun problema
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
