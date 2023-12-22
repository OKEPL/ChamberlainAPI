#region

using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.AppServer.Api.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

#endregion

namespace Chamberlain.AppServer.Api.Endpoint
{
    #region

    using System;
    using System.IO;
    using System.Text;

    using Akka.Actor;
    using Akka.Configuration;

    using Chamberlain.AppServer.Api.Endpoint.Helpers;
    using Chamberlain.AppServer.Api.Endpoint.Infrastructure;
    using Chamberlain.AppServer.Api.Endpoint.Models;
    using Chamberlain.ExternalServices.Email;

    using global::AppServer.Api.Endpoint.Controllers;
    using global::AppServer.Api.Endpoint.Infrastructure;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    using Swashbuckle.AspNetCore.Swagger;

    #endregion

    /// <summary>
    /// Start
    /// </summary>
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        /// <summary>
        /// Configuration to start
        /// </summary>
        public Startup(IHostingEnvironment env)
        {
            this._env = env;

            var builder = new ConfigurationBuilder().SetBasePath(this._env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile(
                    $"appsettings.{env.EnvironmentName}.json",
                    optional: true);

            this.Configuration = builder.Build();

            Common.Content.StructureMapContent.ObjectFactory.Reset();

            Common.Content.StructureMapContent.ObjectFactory.Container.Configure(x =>
            {
                x.AddRegistry(new ApiEndpointRegistry());

            });

        }

        /// <summary>
        /// The Interface IConfigure get Configuration 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// The configure environment
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<ActorBootstrapper>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSimpleCors();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v2/swagger.json", "Solomio API v2"); });

            app.UseAuthentication();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();
        }

        /// <summary>
        /// The configure services
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsEnvironment("Testing"))
            {
                services.AddDbContext<ApplicationDbContext>();
            }
            else
            {
                var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>();
                dbContextOptions.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"));
                dbContextOptions.UseLazyLoadingProxies();

                services.AddDbContext<ApplicationDbContext>(
                    options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"));
                        options.UseLazyLoadingProxies();
                    });
                new ApplicationDbContext(dbContextOptions.Options).Database.Migrate();
            }
            
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IStbService, StbService>();

            services.AddSingleton(this.Configuration);

            services.AddSwaggerGen(
                c =>
                    {
                        c.SwaggerDoc("v2", new Info { Title = "Solomio API", Version = "v2" });
                        var basePath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basePath, "AppServer.Api.Endpoint.xml");
                        c.IncludeXmlComments(xmlPath);
                    });

            services.ConfigureSwaggerGen(
                options => { options.OperationFilter<AuthorizationHeaderParameterOperationFilter>(); });

            services.AddMvc(
                opt => { opt.UseCentralRoutePrefix(new RouteAttribute(ChamberlainBaseController.BaseRouting)); });

            services.Configure<IdentityOptions>(options => { options.User.RequireUniqueEmail = true; });

            this.ConfigureSecurity(services);
            services.AddSingleton(
                provider =>
                    {
                        var cfg = File.ReadAllText(Path.Combine(this._env.ContentRootPath, "EndpointSystem.hocon"));
                        cfg = cfg.Replace("#hostname", this.Configuration["Akka:Hostname"]).Replace(
                            "#port",
                            this.Configuration["Akka:Port"]);

                        var system = ActorSystem.Create("ApiEndpoint", ConfigurationFactory.ParseString(cfg));
                        return system;
                    });

            services.AddSingleton<ActorBootstrapper>();
        }

        private void ConfigureSecurity(IServiceCollection services)
        {
            services.AddAuthentication().AddCookie(cfg => cfg.SlidingExpiration = true).AddJwtBearer(
                cfg =>
                    {
                        cfg.RequireHttpsMetadata = false;
                        cfg.SaveToken = true;

                        cfg.TokenValidationParameters = new TokenValidationParameters()
                                                            {
                                                                ValidateIssuer = true,
                                                                ValidateAudience = true,
                                                                ClockSkew = TimeSpan.Zero,
                                                                RequireExpirationTime = false,
                                                                ValidIssuer = this.Configuration["Tokens:Issuer"],
                                                                ValidAudience = this.Configuration["Tokens:Issuer"],
                                                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"])),
                                                            };
                    });

            services.AddRouting();
            services.AddAuthorization();
        }
    }
}