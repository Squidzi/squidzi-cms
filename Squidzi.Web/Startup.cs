using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Squidzi.Services.CMS.Contracts;
using Squidzi.Services.CMS;
using Squidzi.Services.CMS.Mappers.Contracts;
using Squidzi.Infrastructure.Configuration;
using Squidzi.Infrastructure.Cache;
using Squidzi.Domain.SquidexRepo.Contracts;
using Squidzi.Domain.SendGridRepo.Contracts;
using Squidzi.Domain.SquidexRepo;
using Squidzi.Domain.SendGridRepo;
using Squidzi.Services.CMS.Mappers;
using Squidzi.Services.Email.Contracts;
using Squidzi.Services.Email;
using Microsoft.AspNetCore.ResponseCompression;
using Squidzi.Infrastructure.Cache.Contracts;
using Squidzi.Services.Managers.Contracts;
using Squidzi.Services.Managers;
using Microsoft.Net.Http.Headers;

namespace Squidzi.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            #region Configuration

            services.Configure<SquidexSettings>(_configuration.GetSection("Squidex"));
            services.Configure<CacheSettings>(_configuration.GetSection("Cache"));
            services.Configure<EmailSettings>(_configuration.GetSection("Email"));
            services.Configure<DisqusSettings>(_configuration.GetSection("Disqus"));

            #endregion

            #region Services

            // Infrastructure Services
            services.AddHttpClient();
            services.AddApplicationInsightsTelemetry();

            // Caching
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheProvider, MemoryCacheProvider>();

            // Business Logic Services
            services.AddSingleton<ICMSMapper, CMSMapper>();

            services.AddSingleton<ICMSService, CMSService>();
            services.AddSingleton<IEmailService, EmailService>();

            // Business Logic Managers
            services.AddTransient<ICacheManager, CacheManager>();
            services.AddScoped<IMetaTagsManager, MetaTagsManager>();

            // Business Logic Service Repos
            services.AddSingleton<ISquidexRepo, SquidexRepo>();
            services.AddSingleton<ISendGridRepo, SendGridRepo>();

            #endregion

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml", "application/wasm" });
                options.EnableForHttps = true; // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression#compression-with-secure-protocol
            });
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

            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + ApplicationConstants.StaticFileCachingSeconds;
                }
            });
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }
    }
}
