using LibraryModels.Repository;
using LocalApi.Service;
using ActiveApiHH.ru;
using Microsoft.Extensions.Configuration;



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
builder.Services.AddScoped<IConfigurationRoot>(x => config);

// ������ ��������
builder.Services.AddScoped<IHandler, Handler>();

//������ ����������� => ����� Dapper � �������
builder.Services.AddTransient<IRepositoryDapper<Loggs>>(x => new RepositoryDapper(config["StringConnectForDapper"]));

//������� ������������� ����� EF
builder.Services.AddTransient<IRepository>(x => new Repository(config["StringConnectForEF"]));
builder.Services.AddTransient<IRepositoryExtra>(x => new Repository(config["StringConnectForEF"]));

// ��������� ������ ��� ���������� ���
builder.Services.AddScoped<IActiveForApi, ActiveForApi>();

// ���. ������ ��� Refresh_Token
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
