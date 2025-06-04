using ClickFlow.BLL.DTOs.ConversionDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ConversionController : BaseAPIController
	{
		private readonly IConversionService _conversionService;

		public ConversionController(IConversionService conversionService)
		{
			_conversionService = conversionService;
		}
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] ConversionCreateDTO dto)
		{
			if (!ModelState.IsValid)
				return ModelInvalid();

			try
			{
				var result = await _conversionService.CreateAsync(dto);
				return SaveSuccess(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return SaveError();
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] ConversionGetAllDTO dto)
		{
			try
			{
				var result = await _conversionService.GetAllAsync(dto);
				return GetSuccess(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return GetError();
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var result = await _conversionService.GetByIdAsync(id);
				if (result == null)
					return GetNotFound($"Conversion with ID {id} not found");

				return GetSuccess(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return GetError();
			}
		}

		[HttpPut("{id}/status")]
		public async Task<IActionResult> UpdateStatus(int id, [FromBody] ConversionUpdateStatusDTO dto)
		{
			if (!ModelState.IsValid)
				return ModelInvalid();

			try
			{
				var result = await _conversionService.UpdateStatusAsync(id, dto);
				return SaveSuccess(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return SaveError();
			}
		}


	}
}
