using Alipay.AopSdk.AspnetCore;
using MedicalEquipment_Sharing.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalEquipment_Sharing
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
			services.AddAlipay(options =>
			{
				options.SetOption(Configuration.GetSection("AlipayConfig").Get<AlipayOptions>());
			});

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsProduction())
			{
				app.UseForwardedHeaders(new ForwardedHeadersOptions
				{
					ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
				});

				app.UseExceptionHandler("/Error");

				app.UseMiddleware<JumpErrorPage>();
			}
			else
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes => 
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

		}
	}
}
