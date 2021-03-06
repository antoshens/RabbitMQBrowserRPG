using FightServiceAPI.EventHandlers;
using FightServiceAPI.Data;
using FightServiceAPI.Events;
using FightServiceAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Core.Bus;
using RabbitMQ.Infrastructure.IoC;
using MediatR;

namespace FightServiceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTransient<AttackCreatedHandler>();
            services.AddTransient<IEventHandler<AttackCreatedEvent>, AttackCreatedHandler>();

            services.AddTransient<AttackCreatedRPCHandler>();
            services.AddTransient<IRPCEventHandler<AttackCreatedEvent>, AttackCreatedRPCHandler>();

            services.AddScoped<IAttackService, AttackService>();

            services.AddMediatR(typeof(Startup));

            RegisterServices(services);

            /* MS SQL Server connection */
            services.AddDbContext<AttackDataContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
            );
        }

        private void RegisterServices(IServiceCollection services)
        {
            DependencyContainer.RegisterServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigureEnventBus(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //ConfigureEnventBus(app);
        }

        private void ConfigureEnventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.RPCSubscribe<AttackCreatedEvent, AttackCreatedRPCHandler, AttackFinishedEvent>();
        }
    }
}
