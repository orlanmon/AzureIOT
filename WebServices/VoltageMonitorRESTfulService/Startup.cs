using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Swagger;
using VoltageMonitor.dtos;


namespace VoltageMonitorRESTfulService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();


            // Configure AutoMapper For Model to DTOs
           AutoMapperConfiguration.Configure();

        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Dependency Injection
            //services.AddTransient<IBookService, BookService>();

            // Dependency Injection 
            //services.AddScoped

            // Configuration Settings

            // Add functionality to inject IOptions<T>
            //services.AddOptions();

            // Add our IOpions Config object so it can be injected
            //services.Configure<RESTfulCoreService.Helpers.ApplicationConfiguration>(Configuration.GetSection("ApplicationSettings"));

            // Add the configuration singleton here
            //services.AddSingleton<IConfiguration>(Configuration);


            // Require Authenticated User
            services.AddAuthorization(options => {
                options.AddPolicy("BasicAuthentication", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);

                });
            });




            // Register the Swagger generator, defining one or more Swagger documents
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "ASP.NET Core Web API",
                    Version = "v1",
                    Description = "ASP.NET Core Web API for Voltage Monitoring Device",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Orlando Monaco",
                        Email = string.Empty,
                        Url = "https://www.monacos.us"
                    }
                }
                );
            });
            



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddAzureWebAppDiagnostics();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            loggerFactory.AddApplicationInsights(app.ApplicationServices);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });





            // Authority: AAD + Tenant
            // Audience: Receiver of OpenIDConnect Tokens

            /*
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"],
                Audience = Configuration["Authentication:AzureAD:Audience"],
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer =
                    Configuration["Authentication:AzureAd:AADInstance"]
                  + Configuration["Authentication:AzureAd:TenantId"] + "/ v2.0"
                }

            });
            */

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller}/{action}/{id?}");
               
            });


        }



    }
}
