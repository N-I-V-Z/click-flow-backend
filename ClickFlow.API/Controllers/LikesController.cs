using ClickFlow.BLL.DTOs.LikeDTOs;
using ClickFlow.BLL.DTOs.Response;
using ClickFlow.BLL.Services.Interfaces;
using ClickFlow.DAL.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClickFlow.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class LikesController : BaseAPIController
	{
		private readonly ILikeService _likeService;

		public LikesController(ILikeService likeService)
		{
			_likeService = likeService;
		}

		[HttpPost("like/{postId}")]
		public async Task<ActionResult<BaseResponse>> LikePost(int postId)
		{	
			var result = await _likeService.LikePost(postId, UserId);
			return Ok(result);
		}

		[HttpDelete("unlike/{postId}")]
		public async Task<ActionResult<BaseResponse>> UnlikePost(int postId)
		{		
			var result = await _likeService.UnlikePost(postId, UserId);
			return Ok(result);
		}

		[HttpGet("check/{postId}")]
		public async Task<ActionResult<bool>> IsPostLiked(int postId)
		{			
			var result = await _likeService.IsPostLikedByUser(postId, UserId);
			return Ok(result);
		}

		[HttpGet("post/{postId}")]
		[AllowAnonymous]
		public async Task<ActionResult<PaginatedList<LikeResponseDTO>>> GetLikesByPostId(
			int postId, 
			[FromQuery] int pageIndex = 1, 
			[FromQuery] int pageSize = 10)
		{
			var result = await _likeService.GetLikesByPostId(postId, pageIndex, pageSize);
			return Ok(result);
		}

		[HttpGet("count/{postId}")]
		[AllowAnonymous]
		public async Task<ActionResult<int>> GetLikeCount(int postId)
		{
			var result = await _likeService.GetLikeCountByPostId(postId);
			return Ok(result);
		}

	}
} 