using ClickFlow.BLL.Helpers.Mapper;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Repositories;
using ClickFlow.DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
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

		//add auto mapper
		public static void AddMapper(this IServiceCollection services)
		{
			services.AddAutoMapper(typeof(MappingProfile));
		}

		//Seed Data
		public static async Task SeedData(this IServiceProvider serviceProvider)
		{
			
		}
	}
}
