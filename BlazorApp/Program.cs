using DbDesign.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoViDAccountant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbDesign.CoViDAccountantDbContext>();

                var saveChanges = DbDesign.SeedData.SeedIdentityRoles(
                    scope.ServiceProvider.GetService<UserManager<User>>(),
                    scope.ServiceProvider.GetService<RoleManager<Role>>())
                .GetAwaiter().GetResult();
                
                if(saveChanges)
                    dbContext.SaveChanges();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://0.0.0.0:4800",
                    //                   "https://0.0.0.0:4801");
                });
    }
}
