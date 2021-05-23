namespace Blazor.Contentful.Blog.Starter
{
    using System.Collections.Generic;
    using System.Globalization;
    using Blazor.Contentful.Blog.Starter.Contentful.Api;
    using Blazor.Contentful.Blog.Starter.Contentful.GraphQL;
    using Blazor.Contentful.Blog.Starter.Data;
    using Blazor.Contentful.Blog.Starter.SitemapGeneration.Api;
    using Blazor.Contentful.Blog.Starter.SitemapGeneration.Generators;
    using global::Contentful.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // TODO: Implement WebHook listener
            // This will be used to cache bust the Content/Blogs

            // Setup Sitemap.xml
            services.AddSingleton<SitemapGenerator, DynamicSitemapGenerator>();

            // I18n Services
            services
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .Configure<RequestLocalizationOptions>(
                    opts =>
                    {
                        var supportedCultures = new List<CultureInfo>
                        {
                            // Set Supported Locales
                            new CultureInfo("en-US"),
                        };

                        opts.DefaultRequestCulture = new RequestCulture("en-US");
                        // Formatting numbers, dates, etc.
                        opts.SupportedCultures = supportedCultures;
                        // UI strings that we have localized.
                        opts.SupportedUICultures = supportedCultures;
                    }
                );

            services.Configure<SiteConfig>(Configuration);
            services.AddContentful(Configuration);
            services.AddSingleton<ContentfulApi, GraphQLContentfulApi>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapGet("/sitemap.xml", async context =>
                {
                    await context.Response.WriteAsync(
                        await context.RequestServices
                            .GetRequiredService<SitemapGenerator>()
                            .Generate()
                    );
                });
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
