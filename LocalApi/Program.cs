using LibraryModels.Repository;
using LocalApi.Service;
using ActiveApiHH.ru;
using Microsoft.Extensions.Configuration;



var builder = WebApplication.CreateBuilder(args);

// получаем конфиг. данные для Dapper и Ef
var config = new ConfigurationBuilder().
    AddJsonFile("appsettings.json").
    Build();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// сервис для использования данных из настроечного файла в приложении
builder.Services.AddScoped<IConfigurationRoot>(x => config);

// сервис помощник
builder.Services.AddScoped<IHandler, Handler>();

//сервис логирования => через Dapper в таблицу
builder.Services.AddTransient<IRepositoryDapper<Loggs>>(x => new RepositoryDapper(config["StringConnectForDapper"]));

//сервисы реализованные через EF
builder.Services.AddTransient<IRepository>(x => new Repository(config["StringConnectForEF"]));
builder.Services.AddTransient<IRepositoryExtra>(x => new Repository(config["StringConnectForEF"]));

// добавляем сервис для стороннего АПИ
builder.Services.AddScoped<IActiveForApi, ActiveForApi>();

// доп. сервис для Refresh_Token
builder.Services.AddTransient<IRefresh_token, Refresh_token>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();


app.MapControllerRoute("default", "{controller}/{action}");

app.Run();
