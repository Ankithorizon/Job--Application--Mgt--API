using EF.Core.Job.Application.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLib.Job.Application.Interfaces;
using ServiceLib.Job.Application.Repositories;
using ResumeService.Job.Application.Interfaces;
using ResumeService.Job.Application.Repositories;
using EmailService.Job.Application;
using EmailService.Job.Application.Interfaces;
using EmailService.Job.Application.Repositories;

namespace API.Job.Application
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

            #region resume-service
            services.AddScoped<IResumeCreator, ResumeCreator>();
            #endregion

            #region email-service
            var emailConfig = Configuration
                            .GetSection("EmailConfiguration")
                            .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();
            #endregion

            #region email-file-attachment
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            #endregion     

            #region Repositories
            services.AddTransient<IJobApplicationRepository, JobApplicationRepository>();
            services.AddTransient<IJobResumeRepository, JobResumeRepository>();

            #endregion

            #region JobApplicationDBContext
            services.AddDbContext<JobApplicationDBContext>(options =>
                    options.UseSqlServer(
                      Configuration.GetConnectionString("DefaultConnection"),
                      b => b.MigrationsAssembly(typeof(JobApplicationDBContext).Assembly.FullName)));
            #endregion

            #region cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseCors("CorsPolicy");


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
