using API.Data.Model;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockTrader.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StockTrader
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false).Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(op => op.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly)));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(op =>
                {
                    op.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddOperationalStore(op =>
                {
                    op.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                })
                .AddProfileService<IdentityProfileService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                InitializeDatabase(app);
            }
            app.UseIdentityServer();
        }
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                //var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //var identityContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                foreach (var client in Config.GetClients())
                {
                    var clientInDb = context.Clients
                        .Include(c=>c.AllowedScopes)
                        .Where(c => c.ClientId == client.ClientId).FirstOrDefault();
                    var entity = client.ToEntity();
                    if (clientInDb != null)
                    {
                        clientInDb.AllowedScopes = entity.AllowedScopes;
                        clientInDb.AccessTokenLifetime = entity.AccessTokenLifetime;
                    }
                    else
                    {
                        context.Clients.Add(entity);
                    }
                }
                context.SaveChanges();



                foreach (var resource in Config.IdentityResources())
                {
                    var resourceInDb = context.IdentityResources.FirstOrDefault(c => c.Name == resource.Name);
                    var entity = resource.ToEntity();
                    if (resourceInDb != null)
                    {
                        resourceInDb.Name = entity.Name;
                        resourceInDb.DisplayName = entity.DisplayName;
                        resourceInDb.Enabled = entity.Enabled;
                    }
                    else
                    {
                        context.IdentityResources.Add(entity);
                    }
                }
                context.SaveChanges();



                foreach (var resource in Config.GetAllApiResources())
                {
                    var apiResourceInDb = context.ApiResources.FirstOrDefault(c => c.Name == resource.Name);
                    var entity = resource.ToEntity();
                    if (apiResourceInDb != null)
                    {
                        apiResourceInDb.Name = entity.Name;
                        apiResourceInDb.LastAccessed = entity.LastAccessed;
                        apiResourceInDb.NonEditable = entity.NonEditable;
                        apiResourceInDb.Secrets = entity.Secrets;
                        apiResourceInDb.ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument;
                        apiResourceInDb.Updated = entity.Updated;
                        apiResourceInDb.UserClaims = entity.UserClaims;
                    }
                    else
                    {
                        context.ApiResources.Add(entity);
                    }
                }
                context.SaveChanges();

                foreach (var scope in Config.Scopes())
                {
                    var scopeInDb = context.ApiScopes.FirstOrDefault(c=>c.Name == scope.Name);
                    var entity = scope.ToEntity();
                    if (scopeInDb != null)
                    {
                        scopeInDb.Name = entity.Name;
                        scopeInDb.Properties = entity.Properties;
                        scopeInDb.Required = entity.Required;
                        scopeInDb.ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument;
                        scopeInDb.UserClaims = entity.UserClaims;
                    }
                    else
                    {
                        context.ApiScopes.Add(entity);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
