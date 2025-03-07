using ClickFlow.BLL.Helpers.Mapper;
using ClickFlow.DAL.EF;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Enums;
using ClickFlow.DAL.Repositories;
using ClickFlow.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ClickFlow.API.ConfigExtensions
{
	public static class ServiecsExtensions
	{
		//Unit Of Work
		public static void AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}

		//RepoBase
		public static void AddRepoBase(this IServiceCollection services)
		{
			services.AddScoped(typeof(IRepoBase<>), typeof(RepoBase<>));
		}

		//BLL Services
		public static void AddBLLServices(this IServiceCollection services)
		{
			services.Scan(scan => scan
					.FromAssemblies(Assembly.Load("ClickFlow.BLL"))
					.AddClasses(classes => classes.Where(type => type.Namespace == $"ClickFlow.BLL.Services.Implements" && type.Name.EndsWith("Service")))
					.AsImplementedInterfaces()
					.WithScopedLifetime());
		}

		// Auto mapper
		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MappingProfile));
		}

		// Swagger
		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(option =>
			{
				option.SwaggerDoc("v1", new OpenApiInfo { Title = "ClickFlow API", Version = "v1" });
				option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				option.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type=ReferenceType.SecurityScheme,
								Id="Bearer"
							}
						},
						new string[]{}
					}
				});
			});
		}

		//Seed Data
		public static async Task SeedData(this IServiceProvider serviceProvider)
		{
			using var context = new ClickFlowContext(serviceProvider.GetRequiredService<DbContextOptions<ClickFlowContext>>());

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();

			// Lấy môi trường hiện tại (Development, Production)
			var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

			if (!context.Roles.Any())
			{
				context.Roles.AddRange(
					new IdentityRole<int> { Name = "Admin", NormalizedName = "ADMIN" },
					new IdentityRole<int> { Name = "Advertiser", NormalizedName = "ADVERTISER" },
					new IdentityRole<int> { Name = "Publisher", NormalizedName = "PUBLISHER" }
				);
				context.SaveChanges();
			}

			if (env.IsDevelopment())
			{
				if (!context.Publishers.Any())
				{
					context.Publishers.AddRange(
						new Publisher()
					);
					context.SaveChanges();
				}

				if (!context.Advertisers.Any())
				{
					context.Advertisers.AddRange(
						new Advertiser { CompanyName = "ABC", IntroductionWebsite = "ABC", StaffSize = 0, Industry = Industry.FoodAndBeverage }
					);
					context.SaveChanges();
				}

				if (!context.Users.Any())
				{
					context.Users.AddRange(
						// Pass: Admin@123
						new ApplicationUser { FullName = "admin", Role = Role.Admin, UserName = "admin", NormalizedUserName = "ADMIN", Email = "admin@email.com", NormalizedEmail = "ADMIN@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEDH0xTQNvAznmb/NtaE+zrtLrV4Xz1hGMInXCZE2MoDFR88A06IT6meJb7wHSEj6vQ==", SecurityStamp = "BWYPPRX7FGAHVOE7REDRNSWC72LU67ZP", ConcurrencyStamp = "4bd4dcb0-b231-4169-93c3-81f70479637a", PhoneNumber = "0999999999", LockoutEnabled = true },
						// Pass: Publisher@123
						new ApplicationUser { FullName = "publisher", Role = Role.Publisher, PublisherId = 1, UserName = "publisher", NormalizedUserName = "PUBLISHER", Email = "publisher@email.com", NormalizedEmail = "PUBLISHER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEOt/MXLgdzJxojqjsq7hJ555rrtf1O8cKdPgrxcQ6qcktsP1W0eEaDzAdlWJDclDkw==", SecurityStamp = "VPTXDWZBUHO7WKLE7YSJEVUEA2VFKO3Q", ConcurrencyStamp = "74aa5c69-1d53-452a-9e65-ed467f5c08a7", PhoneNumber = "0988888888", LockoutEnabled = true },
						// Pass: Advertiser@123
						new ApplicationUser { FullName = "advertiser", Role = Role.Advertiser, AdvertiserId = 1, UserName = "advertiser", NormalizedUserName = "ADVERTISER", Email = "advertiser@email.com", NormalizedEmail = "ADVERTISER@EMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEAN3E2E/F3buDNQ0SZqFsAKaBHiyju1qtHw9xHXA/GRgPrTykm9xzk2/cZbxBtcyJQ==", SecurityStamp = "T2TN5HE4A5KPDBID35DKS2K5EL2PGOO4", ConcurrencyStamp = "b7864ba2-e029-4005-84aa-96000f2044de", PhoneNumber = "0977777777", LockoutEnabled = true }
					);
					context.SaveChanges();

				}
			}

		}
	}
}
