using ActiveApiHH.ru;
using Hangfire;
using Hangfire.SqlServer;
using LibraryModels.Repository;
using LocalApi.Service;



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
//!!Переделал=>hangfire некорректно работал при Scoped
builder.Services.AddTransient<IConfigurationRoot>(x => config);

// сервис помощник
builder.Services.AddScoped<IHandler, Handler>();

//сервис логирования => через Dapper в таблицу
builder.Services.AddTransient<IRepositoryDapper<Loggs>>(x => new LoggerRepositoryDapper(config["StringConnectForDapper"]));

//сервисы реализованные через EF
builder.Services.AddTransient<IRepository>(x => new Repository(config["StringConnectForEF"]));
builder.Services.AddTransient<IRepositoryExtra>(x => new Repository(config["StringConnectForEF"]));

// добавляем сервис для стороннего АПИ
builder.Services.AddTransient<IActiveForApi, ActiveForApi>();

// доп. сервис для Refresh_Token
builder.Services.AddTransient<IRefresh_token, Refresh_token>();

// добавляем настройки БД для Hangfire
builder.Services.AddHangfire(configuration => configuration.
        SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(config["StringConnectForHangfire"], new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

//добавляем сервис по запуску сервера
builder.Services.AddHangfireServer();

// добавляем сервис для работы с Hangfire
builder.Services.AddTransient<IServiceForHangfire, ServiceForHangfire>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // добавляем интерфейс
    app.UseHangfireDashboard();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();


app.MapControllerRoute("default", "{controller}/{action}");

app.Run();
