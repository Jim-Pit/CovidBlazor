using CoViDAccountant.Data;
using CoViDAccountant.Services;
using DbDesign;
using DbDesign.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //services.AddSingleton<WeatherForecastService>();

            services.AddSingleton<UIService>();
            services.AddScoped<DiagnosticCenterManagementService>();
            services.AddScoped<GenericOperationsService>();

            services.AddDbContext<CoViDAccountantDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("CoViDAccountantConn"));
            });

            services.AddIdentity<User, Role>(options =>
                    {
                        //options.User.RequireUniqueEmail = true;
                        options.Password = new PasswordOptions
                        {
                            RequireDigit = true,
                            RequireLowercase = true,
                            RequireUppercase = true,
                            RequireNonAlphanumeric = true,
                            RequiredLength = 5,
                            RequiredUniqueChars = 5,
                        };
                    })
                    .AddEntityFrameworkStores<CoViDAccountantDbContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToAccessDenied =
                    options.Events.OnRedirectToLogin =
                        context =>
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        };
            });
            services.AddAuthentication();
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
