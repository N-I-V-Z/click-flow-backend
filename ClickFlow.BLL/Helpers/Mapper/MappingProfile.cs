using AutoMapper;
using System.Reflection;

namespace ClickFlow.BLL.Helpers.Mapper
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
			var dalAssembly = Assembly.Load("ClickFlow.DAL");
			var bllAssembly = Assembly.Load("ClickFlow.BLL");


			var entityTypes = dalAssembly.GetTypes().Where(t => t.IsClass && t.Namespace == "ClickFlow.DAL.Entities");


			foreach (var entityType in entityTypes)
			{
				var dtoTypes = bllAssembly.GetTypes()
					.Where(t => t.IsClass && t.Namespace == $"ClickFlow.BLL.DTOs.{entityType.Name}DTOs" && t.Name.StartsWith(entityType.Name));

				foreach (var dtoType in dtoTypes)
				{
					CreateMap(entityType, dtoType).ReverseMap();
				}
			}
        }
    }
}
