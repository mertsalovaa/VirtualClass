using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VirtualClass.DAL;
using VirtualClass.DAL.Entities;
using VirtualClass.DAL.Helper;
using VirtualClass.DLL.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace VirtualClass
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
            services.AddDbContext<EFContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<DbUser, DbRole>()
                .AddEntityFrameworkStores<EFContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
            });

            services.AddTransient<IJWTTokenService, JWTTokenService>();

            var jwtTokenSecretKey = Configuration.GetValue<string>("SecretPhrase");
            var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSecretKey));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = signInKey,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // set ClockSkew is zero
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client-app/build";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "YPS API",
                    Description = "A project  ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Team YPS",
                        Email = string.Empty,
                    },

                });

                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Deadlines.API v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "client-app";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            SeederDatabase.SeedData(app.ApplicationServices, Configuration);
        }
    }
}

