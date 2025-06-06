using ClickFlow.BLL.DTOs.PostDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostsController : BaseAPIController
	{
		private readonly IPostService _postService;

		public PostsController(IPostService postService)
		{
			_postService = postService;
		}


		[Authorize]
		[HttpGet]
		[Route("get-all-posts/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetAllPosts(int pageIndex, int pageSize)
		{
			var response = await _postService.GetAllPosts(pageIndex, pageSize);
			return GetSuccess(response);
		}


		[Authorize]
		[HttpGet]
		[Route("get-posts-by-author/{authorId}/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetPostsByAuthorId(int authorId, int pageIndex, int pageSize)
		{
			var response = await _postService.GetPostsByAuthorId(authorId, pageIndex, pageSize);
			return GetSuccess(response);
		}


		[Authorize]
		[HttpGet]
		[Route("get-post-by-id/{id}")]
		public async Task<IActionResult> GetPostById(int id)
		{
			var response = await _postService.GetPostById(id);
			if (response == null) return GetError("Bài viết không tồn tại.");
			return GetSuccess(response);
		}


		[Authorize]
		[HttpPost]
		[Route("create-post")]
		public async Task<IActionResult> CreatePost(PostCreateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();
			var response = await _postService.CreatePost(dto, UserId);
			if (!response.IsSuccess) return SaveError(response);
			return SaveSuccess(response);
		}


		[Authorize]
		[HttpPut]
		[Route("update-post")]
		public async Task<IActionResult> UpdatePost(PostUpdateDTO dto)
		{
			if (!ModelState.IsValid) return ModelInvalid();
			var response = await _postService.UpdatePost(dto, UserId);
			if (!response.IsSuccess) return SaveError(response);
			return SaveSuccess(response);
		}


		[Authorize]
		[HttpDelete]
		[Route("delete-post/{id}")]
		public async Task<IActionResult> DeletePost(int id)
		{
			var response = await _postService.DeletePost(id);
			if (!response.IsSuccess) return SaveError(response);
			return SaveSuccess(response);
		}
	}
}
