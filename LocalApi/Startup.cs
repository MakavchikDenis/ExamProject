using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using LibraryModels.Repository;
using LocalApi.Service;
using ActiveApiHH.ru;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace ASB.Insurance.Komplat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var config = new ConfigurationBuilder().
            AddJsonFile("appsettings.json").
            Build();



            services.AddControllers();
            //services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpContextAccessor();

            // сервис для использования данных из настроечного файла в приложении
            services.AddScoped<IConfigurationRoot>(x => config);

            // сервис помощник
            services.AddScoped<IHandler, Handler>();

            //сервис логирования => через Dapper в таблицу
            services.AddTransient<IRepositoryDapper<Loggs>>(x => new RepositoryDapper(config["StringConnectForDapper"]));

            //сервисы реализованные через EF
            services.AddTransient<IRepository>(x => new Repository(config["StringConnectForEF"]));
            services.AddTransient<IRepositoryExtra>(x => new Repository(config["StringConnectForEF"]));

            // добавляем сервис для стороннего АПИ
            services.AddScoped<IActiveForApi, ActiveForApi>();



            //var settingConnectionString = new SettingConnectionString();
            //Configuration.Bind("ConnectionStrings", settingConnectionString);
            //services.AddSingleton(settingConnectionString);
            //services.AddTransient<InsuranceDBContent>(s => new InsuranceDBContent(settingConnectionString));
            //services.AddTransient<APIRepository.IDataContextRepository, APIRepository.DataContextRepository>();
            //services.AddMvc().AddSessionStateTempDataProvider().AddXmlSerializerFormatters();
            //services.AddControllers(options =>
            //{
            //    options.RespectBrowserAcceptHeader = true;
            //}).AddXmlSerializerFormatters().AddNewtonsoftJson();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ASB.Insurance.Komplat", Version = "v1" });
            //});
            //services.AddLogging(builder =>
            //{
            //    builder.SetMinimumLevel(LogLevel.Trace);
            //    builder.AddFilter("Microsoft", LogLevel.Warning);
            //    builder.AddFilter("System", LogLevel.Error);
            //    builder.AddFilter("Engine", LogLevel.Warning);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASB.Insurance.Komplat v1"));
            }

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller}/{action}");
            });
            
        }
    }
}
