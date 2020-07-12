using Coiner.Business;
using Coiner.Business.Context;
using Coiner.Business.Heplers;
using Coiner.Business.Models;
using log4net;
using log4net.Config;
using MangoPay.SDK;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Coiner
{
    public class Startup
    {
        private IConfiguration config;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment.WebRootPath = environment.WebRootPath;

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p =>
                {
                    p.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            services.AddSignalR();
            services.AddMvc();
            services.AddNodeServices();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist/browser";
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = "yourdomain.com",
                      ValidAudience = "yourdomain.com",
                      IssuerSigningKey = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes("My_Keydqsdsqdqsdqsdqsdqs"))
                  };
              });

            //MangoPay API
            //services.AddSingleton<IMangoPayApi, MangoPayApi>(serviceProvider =>
            //{
  
            //});

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", true, true)
                                  .Build();
            var AdminConfig = config.GetSection("AdminConfig");
            var ComissionConfig = config.GetSection("ComissionConfig");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                CoinerContext db = new CoinerContext();
                if (!db.Users.Any())
                {
                    var admin = new User()
                    {
                        Email = Convert.ToString(AdminConfig["AdminMail"]),
                        Login = Convert.ToString(AdminConfig["AdminLogin"]),
                        Password = Convert.ToString(AdminConfig["AdminPassword"]),
                        PhoneNumber = Convert.ToString(AdminConfig["AdminPhoneNumber"]),
                        FirstName = Convert.ToString(AdminConfig["AdminFirstName"]),
                        LastName = Convert.ToString(AdminConfig["AdminLastName"]),
                        Job = "Admin",
                        IsActive = Business.Models.Enums.AccountVerificationEnum.Verified,
                        Gender = Business.Models.Enums.GenderEnum.Male
                    };
                    db.Users.Add(admin);
                    db.SaveChanges();

                    var comission = new User()
                    {
                        Email = Convert.ToString(ComissionConfig["ComissionMail"]),
                        Login = Convert.ToString(ComissionConfig["ComissionLogin"]),
                        Password = Convert.ToString(ComissionConfig["ComissionPassword"]),
                        PhoneNumber = Convert.ToString(ComissionConfig["ComissionPhoneNumber"]),
                        FirstName = Convert.ToString(ComissionConfig["ComissionFirstName"]),
                        LastName = Convert.ToString(ComissionConfig["ComissionLastName"]),
                        Job = "Comission",
                        IsActive = Business.Models.Enums.AccountVerificationEnum.Verified,
                        Gender = Business.Models.Enums.GenderEnum.Male
                    };
                    comission.UserCoinsNumber = Constants.DefaultUserCoinsNmber;
                    var userWallet = new UserWallet()
                    {
                        UnusedCoinsNumber = Constants.DefaultUserCoinsNmber
                    };

                    comission.UserWallet = userWallet;
                    db.Users.Add(comission);
                    db.SaveChanges();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseCors(builder =>
                 builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
            //app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<NotifyHub>("/notify");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
            //For angular url rewriting  possible fix for rewrite issue
            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
            //    {
            //        context.Request.Path = "/index.html";
            //        await next();
            //    }
            //});
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";
              
                if (env.IsDevelopment())
                {
                    spa.Options.StartupTimeout = new TimeSpan(0, 0, 80);
                    //spa.UseAngularCliServer(npmScript: "start");
                    // or for ssr 
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
                //else
                //{
                //    spa.UseSpaPrerendering(options =>
                //    {
                //        //options.BootModulePath = $"{spa.Options.SourcePath}/dist/server/main.js";
                //        options.BootModulePath = $"{spa.Options.SourcePath}/dist/server.js";
                //        options.BootModuleBuilder = env.IsDevelopment() ? new AngularCliBuilder(npmScript: "build:ssr && serve:ssr") : null;
                //        //options.BootModuleBuilder = new AngularCliBuilder(npmScript: "build:ssr");
                //        options.ExcludeUrls = new[] { "/sockjs-node" };
                //        //options.SupplyData = (requestContext, obj) =>
                //        //{
                //        //    // var result = appService.GetApplicationData(requestContext).GetAwaiter().GetResult();
                //        //    obj.Add("Cookies", requestContext.Request.Cookies);
                //        //};
                     
                //    });
                //}
            });
        }
    }
}
