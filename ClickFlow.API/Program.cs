
using ClickFlow.API.ConfigExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ClickFlow.DAL.EF;
using System.Text.Json.Serialization;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.BLL.Services.Implements;
using ClickFlow.BLL.Helpers.Config;
using Microsoft.Extensions.Options;
using ClickFlow.DAL.Entities;

namespace ClickFlow.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers()
				.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddDbContext<ClickFlowContext>(option =>
			{
				option.UseSqlServer(builder.Configuration.GetConnectionString("ClickFlowDB"));
			});
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);

            builder.Services.Configure<VnPayConfiguration>(builder.Configuration.GetSection("VnPayConfiguration"));
			builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<VnPayConfiguration>>().Value);



            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<ClickFlowContext>().AddDefaultTokenProviders();

            builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwagger();

			builder.Services.AddCors(opts =>
			{
				opts.AddPolicy("corspolicy", build =>
				{
					build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
				});
			});

			builder.Services.AddRepoBase();

			builder.Services.AddUnitOfWork();

			builder.Services.AddMapper();

          
            builder.Services.AddBLLServices();

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = builder.Configuration["JWT:ValidAudience"],
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
					ClockSkew = TimeSpan.Zero,
					ValidateLifetime = true,
				};
			});

			builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

			builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });


            var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					services.SeedData().Wait();
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while seeding the database.");
				}
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors("corspolicy");

			app.UseHttpsRedirection();

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
