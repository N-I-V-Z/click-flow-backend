using ClickFlow.BLL.DTOs;
using ClickFlow.BLL.DTOs.PlanDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlansController : BaseAPIController
	{
		private readonly IPlanService _planService;

		public PlansController(IPlanService planService)
		{
			_planService = planService;
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] PlanGetAllDTO dto)
		{
			try
			{
				var data = await _planService.GetAllAsync(dto);
				var response = new PagingDTO<PlanResponseDTO>(data);

				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var plan = await _planService.GetByIdAsync(id);

				if (plan == null) return GetNotFound("Không có dữ liệu.");
				return GetSuccess(plan);
			}
			catch (KeyNotFoundException ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] PlanCreateDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var result = await _planService.CreateAsync(dto);

				if (result == null) return SaveError();
				return SaveSuccess(result);
			}
			catch (KeyNotFoundException ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] PlanUpdateDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var updated = await _planService.UpdateAsync(id, dto);

				if (updated == null) return SaveError();
				return SaveSuccess(updated);
			}
			catch (KeyNotFoundException ex)
			{
				return Error(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var success = await _planService.DeleteAsync(id);

				return SaveSuccess(success);
			}
			catch(KeyNotFoundException knfEx)
			{
				return Error(knfEx.Message);
			}
			catch (Exception ex)
			{
				return Error(ex.Message);
			}
		}
	}
}
