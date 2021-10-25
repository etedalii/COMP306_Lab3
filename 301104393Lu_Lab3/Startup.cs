using _301104393Lu_Lab3.Classes;
using _301104393Lu_Lab3.Data;
using _301104393Lu_Lab3.Models;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _301104393Lu_Lab3
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
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            });
            
            var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("connection2RDS"));
            builder.UserID = Configuration["DbUser"];
            builder.Password = Configuration["DbPassword"];
            var connection = builder.ConnectionString;
            services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connection));
            // services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            // AWS Options
            var awsOptions = Configuration.GetAWSOptions();
            services.AddDefaultAWSOptions(awsOptions);

            var client = awsOptions.CreateServiceClient<IAmazonDynamoDB>();
            var dynamoDbOptions = new DynamoDbOptions();
            ConfigurationBinder.Bind(Configuration.GetSection("DynamoDbTables"), dynamoDbOptions);
            services.AddScoped<IDynamoDbContext<Movie>>(provider => new DynamoDbContext<Movie>(client, dynamoDbOptions.Movie));
            services.AddScoped<IDynamoDbContext<Comment>>(provider => new DynamoDbContext<Comment>(client, dynamoDbOptions.Comment));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
