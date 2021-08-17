using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Msoop.Infrastructure.Data;
using Msoop.Web.Extensions;
using Msoop.Web.Reddit;

namespace Msoop.Web
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
            services.AddCookiePolicy(opt =>
            {
                // Consent is needed for Google Analytics
                opt.CheckConsentNeeded = _ => true;
                opt.MinimumSameSitePolicy = SameSiteMode.Strict;
            });
            services.AddDbContext<MsoopContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("MsoopConnection"));
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(Startup));

            services.Configure<RedditOptions>(Configuration.GetSection(RedditOptions.Reddit));
            services.AddRedditService();

            services.AddRazorPages();
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
                // We don't use Hsts and HttpsRedirection middlewares, because
                // our webapp will be hosted behind nginx that will handle it
                // app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseStatusCodePagesWithReExecute("/Status{0}");

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
        }
    }
}
