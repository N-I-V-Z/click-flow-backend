using AutoMapper;
using ClickFlow.BLL.DTOs.ApplicationUserDTOs;
using ClickFlow.BLL.DTOs.CampaignDTOs;
using ClickFlow.BLL.DTOs.CampaignParticipationDTOs;
using ClickFlow.DAL.Entities;
using ClickFlow.DAL.Paging;
using System.Globalization;
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
			CreateMap<CampaignCreateDTO, Campaign>()
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
					DateOnly.FromDateTime(DateTime.ParseExact(src.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))))
				.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
					DateOnly.FromDateTime(DateTime.ParseExact(src.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))));
			CreateMap<CampaignUpdateDTO, Campaign>()
			   .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
				   DateOnly.FromDateTime(DateTime.ParseExact(src.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))))
			   .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src =>
				   DateOnly.FromDateTime(DateTime.ParseExact(src.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))));
			CreateMap<ApplicationUser, ApplicationUserResponseDTO>();
			
			// Thêm mapping cho PaginatedList<CampaignParticipation> sang List<CampaignParticipationResponseDTO>
			CreateMap<PaginatedList<CampaignParticipation>, List<CampaignParticipationResponseDTO>>()
				.ConvertUsing((src, dest, context) => 
				{
					var list = new List<CampaignParticipationResponseDTO>();
					foreach (var item in src)
					{
						list.Add(context.Mapper.Map<CampaignParticipationResponseDTO>(item));
					}
					return list;
				});
				
			// Thêm mapping cho CampaignParticipation sang CampaignParticipationResponseDTO
			CreateMap<CampaignParticipation, CampaignParticipationResponseDTO>()
				.ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.CreateAt)))
				.ForMember(dest => dest.TotalCampaigns, opt => opt.Ignore())
				.ForMember(dest => dest.DailyTraffic, opt => opt.Ignore());

		}
	}
}
