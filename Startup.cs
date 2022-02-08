using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Album2.Models;
using Autofac;
using Microsoft.AspNetCore.Http;

namespace Album2
{
    public class Startup
    {
        string constr;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            constr = Configuration.GetConnectionString("DefaultConnection");
            services.AddTransient<IUserRepository, UserRepository>(provider => new UserRepository(constr));
            services.AddTransient<IRoleRepository, RoleRepository>(provider => new RoleRepository(constr));
            services.AddTransient<IAlbumRepository, AlbumsRepository>(provider => new AlbumsRepository(constr));
            services.AddTransient<IImageRepository, PicsRepository>(provider => new PicsRepository(constr));
            //services.AddTransient<IComRepository, ComRepository>(provider => new ComRepository(constr));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                    opt.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    opt.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Register");
                    opt.Cookie.HttpOnly = true;
                });
            services.AddControllersWithViews();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // autofac
            builder.Register(c => new ComRepository(constr)).As<IComRepository>();
            builder.RegisterType<CryptRepository>().As<ICrypt>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this
                // for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.None,
            };
            app.UseCookiePolicy(cookiePolicyOptions);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
