using System.Net.Http;
using System.Reflection;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestEase;
using TextClustering.Application;
using TextClustering.Application.Algorithm;
using TextClustering.Application.Algorithm.Http;
using TextClustering.Application.Settings;
using TextClustering.Domain;
using TextClustering.Web.Helpers;

namespace TextClustering.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // add services to the DI container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            // configure strongly typed settings object
            IConfigurationSection config = Configuration.GetSection("AppSettings");
            string url = config.Get<AppSettings>().AlgorithmUrl;

            services.Configure<AppSettings>(config);

            services
                .AddMvc(o => { o.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())); })
                .AddJsonOptions(o => o.JsonSerializerOptions.IgnoreNullValues = true);

            // configure DI for application services
            services.AddHttpContextAccessor();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IDatasetClusteringService, DatasetClusteringService>();
            services.AddScoped<IAlgorithmService, AlgorithmService>();

            services.AddScoped(factory =>
            {
                RestClient restClient = new RestClient(new HttpClient(new HttpClientHandler())
                {
                    BaseAddress = new System.Uri(url),
                    Timeout = Timeout.InfiniteTimeSpan
                })
                {
                    JsonSerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                };
                return restClient.For<IClusteringApi>();
            });

            string connectionString = Configuration.GetConnectionString("default");

            services.AddDbContext<TextClusteringDbContext>(options =>
            {
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("TextClustering.Migrations"));
            });

            services.AddAutoMapper(typeof(Startup).GetTypeInfo().Assembly);
        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }
    }
}