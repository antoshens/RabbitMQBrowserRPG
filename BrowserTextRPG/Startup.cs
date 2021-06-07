using BrowserTextRPG.Data;
using BrowserTextRPG.Services.CharacterService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using BrowserTextRPG.Services.WeaponService;
using BrowserTextRPG.Services.FightService;
using BrowserTextRPG.Middlewares;
using RabbitMQ.Infrastructure.IoC;
using MediatR;
using BrowserTextRPG.Events;
using BrowserTextRPG.EventHandlers;
using RabbitMQ.Core.Bus;

namespace BrowserTextRPG
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
            services.AddControllers();

            // Custom services
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IWeaponService, WeaponService>();
            services.AddScoped<IFightService, FightService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<AttackFinishedEventHandler>();
            services.AddTransient<IEventHandler<AttackFinishedEvent>, AttackFinishedEventHandler>();

            services.AddMediatR(typeof(Startup));

            // Register global services via IoC container
            RegisterServices(services);

            /* MS SQL Server connection */
            //services.AddDbContext<DataContext>(options => 
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            //);

            /* SQLite connection */
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqliteConnection"))
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<ExceptionHabdlerMiddleware>();

            // To ignore possible cyclic references
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        private void RegisterServices(IServiceCollection services)
        {
            DependencyContainer.RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHabdlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            this.ConfigureEnventBus(app);
        }

        private void ConfigureEnventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<AttackFinishedEvent, AttackFinishedEventHandler>();
        }
    }
}
