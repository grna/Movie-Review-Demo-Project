using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using SciFiReviewsApi.Models;
using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.OutgoingModels;
using SciFiReviewsApi.Models.ReturnModels;
using SciFiReviewsApi.Services;

namespace SciFiReviewsApi
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
            services.AddDbContext<SciFiReviewsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SciFiReviews")));

            services.AddScoped<IReviewData, SqlReviewData>();
            services.AddScoped<IMovieData, SqlMovieData>();
            services.AddScoped<IReviewerData, SqlReviewerData>();
            services.AddScoped<IUserService, SqlUserService>();
            services.AddScoped<IPosterData, IMPRPosterData>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext =
                implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<ITypeHelperService, TypeHelperService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Review, ReviewReturnModel>()
                    .ForMember(dest => dest.Comments, opt => opt.MapFrom(
                        src => src.Comments.OrderByDescending(c => c.Timestamp)));
                cfg.CreateMap<Movie, MovieReturnModel>();
                cfg.CreateMap<Reviewer, ReviewerReturnModel>()
                    .ForMember(dest => dest.LikedReviews, opt => opt.MapFrom(
                        src => src.LikedReviews.Select(r => int.Parse(r)).ToList()));
                cfg.CreateMap<Comment, CommentReturnModel>()
                    .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(
                        src => src.Reviewer.Username));
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
