﻿using ImageResizer.AspNetCore.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Services.Seed.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MadPay724.Presentation.Helpers.Configuration
{
    public static class InitConfigurationExtensions
    {
        public static void AddMadDbContext(this IServiceCollection services)
        {
            services.AddDbContext<Main_MadPayDbContext>();
            services.AddDbContext<Financial_MadPayDbContext>();
            services.AddDbContext<Log_MadPayDbContext>();
        }
        public static void AddMadInitialize(this IServiceCollection services, int? httpsPort)
        {
            services.AddControllersWithViews();
            services.AddMvcCore(config =>
            {
                config.ReturnHttpNotAcceptable = true;
                config.Filters.Add(typeof(RequireHttpsAttribute));
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
             .AddApiExplorer()
             .AddFormatterMappings()
             .AddDataAnnotations()
             .AddCors(opt =>
             {
                opt.AddPolicy("CorsPolicy", builder =>
                builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
             })
             .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            //
            services.AddResponseCaching();
            services.AddHsts(opt =>
            {
                opt.MaxAge = TimeSpan.FromDays(180);
                opt.IncludeSubDomains = true;
                opt.Preload = true;
            });

            services.AddHttpsRedirection(opt =>
            {
                opt.RedirectStatusCode = StatusCodes.Status302Found;
            });

            //services.AddResponseCompression(opt => opt.Providers.Add<GzipCompressionProvider>());
            //services.AddRouting( opt => opt.LowercaseUrls = true);
            //services.AddApiVersioning(opt =>
            //{
            //    opt.ApiVersionReader = new MediaTypeApiVersionReader();
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //    opt.ReportApiVersions = true;
            //    opt.DefaultApiVersion = new ApiVersion(1,0);
            //    opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            //});
            services.AddImageResizer();
        }

        public static void UseMadInitialize(this IApplicationBuilder app, SeedService seeder)
        {
            //app.UseResponseCompression();
            seeder.SeedUsers();
            app.UseRouting();
            app.UseImageResizer();

            app.UseCsp(opt => opt.DefaultSources(s => s.Self())
            .StyleSources(s=>s.Self().UnsafeInline())
            .ScriptSources(s=>s.Self().UnsafeInline())
            .ImageSources(s => s.Self().CustomSources("res.cloudinary.com", "cloudinary.com", "data:"))
            .MediaSources(s => s.Self().CustomSources("res.cloudinary.com", "cloudinary.com", "data:"))
            .FontSources(s => s.Self().CustomSources("data:"))
            );
            app.UseXfo(o => o.Deny());

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/wwwroot")
            });
        }

        public static void UseMadInitializeInProd(this IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseResponseCaching();
        }
    }
}
