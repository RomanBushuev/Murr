using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Murzik.Entities;
using Murzik.Interfaces;
using Murzik.ReaderMurrData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Murzik.Rest
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
            var mapper = ReaderMurrData.AutoMapperConfiguration.Configure().CreateMapper();
            var dataProvider = Configuration.GetSection("DataProvider").Get<DataProvider>();
            var jsonMapper = AutoMapperConfiguration.Configure().CreateMapper();
            services.AddSingleton<IMapper>(jsonMapper);
            services.AddScoped<IReaderMurrProvider>(z => new ReaderMurrProvider(mapper, dataProvider.KarmaSaver));            

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Murzik.Rest", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Murzik.Rest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
