using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Djelato.Common.Settings;
using Djelato.DataAccess.Managers;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.DataAccess.RedisRepositories;
using Djelato.DataAccess.RedisRepositories.Interfaces;
using Djelato.Services.Notification;
using Djelato.Services.PasswordHasher;
using Djelato.Services.Services;
using Djelato.Services.Services.Interfaces;
using Djelato.Web.Mapping;
using Djelato.Web.Middleware;
using Djelato.Web.ViewModel;
using Djelato.Web.ViewModel.FluentApi;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Djelato.Web
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

            #region Cors policy

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    );
            });

            #endregion

            #region Binding

            services.AddScoped<IMongoManager, MongoManager>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<INotifier, ConcreteEmailNotifier>();

            services.AddSingleton<IHasher, Hasher>();

            services.AddScoped<IRedisRepo, RedisRepo>();

            #endregion

            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<UserValidator>();

                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });

            #region MongoSettings

            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });

            services.AddScoped<IMongoDatabase>(provider =>
            {
                var settings = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(settings.Value.ConnectionString);
                var db = client.GetDatabase(settings.Value.Database);
                return db;
            });

            #endregion

            #region AutoMapper

            MapperConfiguration mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            #endregion

            #region Action model state filter

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var modelStateError = actionContext.ModelState.FirstOrDefault(m => m.Value.ValidationState == ModelValidationState.Invalid);
                    KeyValuePair<string, string> error;
                    error = (modelStateError.Equals(default(KeyValuePair<string, ModelStateEntry>)))
                    ? new KeyValuePair<string, string>()
                    : new KeyValuePair<string, string>(
                        modelStateError.Key,
                        modelStateError.Value.Errors.First().ErrorMessage ?? "the input was not valid"
                        );
                    return new BadRequestObjectResult(error);
                };
            });

            #endregion

            #region MailKit

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            #endregion

            #region Redis

            services.Configure<RedisSettings>(options =>
            {
                options.Host = Configuration.GetSection("RedisConnection:Host").Value;
                options.Port = Configuration.GetSection("RedisConnection:Port").Value;
            });

            services.AddScoped<IConnectionMultiplexer>(provider =>
            {
                var settings = provider.GetService<IOptions<RedisSettings>>();

                IConnectionMultiplexer redisCient = ConnectionMultiplexer.Connect($"{settings.Value.Host}:{settings.Value.Port}");
                return redisCient;
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
