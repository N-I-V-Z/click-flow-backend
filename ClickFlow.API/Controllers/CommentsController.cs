using ClickFlow.BLL.DTOs.CommentDTOs;
using ClickFlow.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClickFlow.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseAPIController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

       
        [HttpPost("create-comment")]
        public async Task<IActionResult> Create([FromBody] CommentCreateDTO dto)
        {
            if (!ModelState.IsValid) return ModelInvalid();
            var response = await _commentService.CreateComment(dto, UserId);
            return response.IsSuccess ? SaveSuccess(response) : SaveError(response);
        }

      
        [HttpPut("update-comment")]
        public async Task<IActionResult> Update([FromBody] CommentUpdateDTO dto)
        {
            if (!ModelState.IsValid) return ModelInvalid();
            var response = await _commentService.UpdateComment(dto, UserId);
            return response.IsSuccess ? SaveSuccess(response) : SaveError(response);
        }

     
        [HttpDelete("delete-comment/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _commentService.DeleteComment(id);
            return response.IsSuccess ? SaveSuccess(response) : SaveError(response);
        }

     
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var comment = await _commentService.GetCommentById(id);
            return comment == null ? GetError("Không tìm thấy bình luận.") : GetSuccess(comment);
        }

    
        [HttpGet("get-comment-by-post-id/{postId}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetByPostId(int postId, int pageIndex, int pageSize)
        {
            var comments = await _commentService.GetCommentsByPostId(postId, pageIndex, pageSize);
            return GetSuccess(comments);
        }
    }
}
