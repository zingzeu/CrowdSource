using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using CrowdSource.Data;
using CrowdSource.Models;
using CrowdSource.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using CrowdSource.Auth;

namespace CrowdSource
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets("aspnet-CrowdSource-840eb12a-4b2d-402c-ba20-082f6ad09a5a");
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build(); 
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options=>
            {
                // Allow weak passwords.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddScoped<IDbConfig, DbConfig>();
            services.AddScoped<IDataLogic, ADFDLogic>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITaskDispatcher, TaskDispatcher>();
            services.AddSingleton<IAnalytics, Analytics>();
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<ITextSanitizer, TextSanitizer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions() {
                FileProvider = new PhysicalFileProvider("/segments"),
                RequestPath = new PathString("/segments")
            });

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            
            
            #region UseJwtBearerAuthentication 
            app.UseJwtBearerAuthentication(new JwtBearerOptions() 
            { 
                TokenValidationParameters = new TokenValidationParameters() 
                { 
                    IssuerSigningKey = TokenAuthOption.Key, 
                    ValidAudience = TokenAuthOption.Audience, 
                    ValidIssuer = TokenAuthOption.Issuer, 
                    // When receiving a token, check that we've signed it. 
                    ValidateIssuerSigningKey = true, 
                    // When receiving a token, check that it is still valid. 
                    ValidateLifetime = true, 
                    // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time  
                    // when validating the lifetime. As we're creating the tokens locally and validating them on the same  
                    // machines which should have synchronised time, this can be set to zero. Where external tokens are 
                    // used, some leeway here could be useful. 
                    ClockSkew = TimeSpan.FromMinutes(0) 
                } 
            }); 
            #endregion 

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
