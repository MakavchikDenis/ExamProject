using ActiveApiHH.ru;
using Hangfire;
using Hangfire.SqlServer;
using LibraryModels.Repository;
using LocalApi.Service;



var builder = WebApplication.CreateBuilder(args);

// �������� ������. ������ ��� Dapper � Ef
var config = new ConfigurationBuilder().
    AddJsonFile("appsettings.json").
    Build();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// ������ ��� ������������� ������ �� ������������ ����� � ����������
//!!���������=>hangfire ����������� ������� ��� Scoped
builder.Services.AddTransient<IConfigurationRoot>(x => config);

// ������ ��������
builder.Services.AddScoped<IHandler, Handler>();

//������ ����������� => ����� Dapper � �������
builder.Services.AddTransient<IRepositoryDapper<Loggs>>(x => new LoggerRepositoryDapper(config["StringConnectForDapper"]));

//������� ������������� ����� EF
builder.Services.AddTransient<IRepository>(x => new Repository(config["StringConnectForEF"]));
builder.Services.AddTransient<IRepositoryExtra>(x => new Repository(config["StringConnectForEF"]));

// ��������� ������ ��� ���������� ���
builder.Services.AddTransient<IActiveForApi, ActiveForApi>();

// ���. ������ ��� Refresh_Token
builder.Services.AddTransient<IRefresh_token, Refresh_token>();

// ��������� ��������� �� ��� Hangfire
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

//��������� ������ �� ������� �������
builder.Services.AddHangfireServer();

// ��������� ������ ��� ������ � Hangfire
builder.Services.AddTransient<IServiceForHangfire, ServiceForHangfire>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // ��������� ���������
    app.UseHangfireDashboard();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();


app.MapControllerRoute("default", "{controller}/{action}");

app.Run();
