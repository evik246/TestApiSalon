using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Comment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.CommentService;

namespace TestApiSalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Roles("Guest", "Client")]
        [HttpGet]
        public async Task<IActionResult> GetAllComments([FromQuery] Paging paging)
        {
            var comments = await _commentService.GetAllComments(paging);
            return comments.MakeResponse();
        }

        [Roles("Guest", "Client")]
        [HttpGet("salon/{id}")]
        public async Task<IActionResult> GetAllCommentsInSalon(int id, [FromQuery] Paging paging)
        {
            var comments = await _commentService.GetAllComments(paging, id);
            return comments.MakeResponse();
        }

        [Roles("Client")]
        [HttpPost("customer/account")]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto request)
        {
            var customerId = this.GetAuthorizedUserId();
            if (customerId.State == ResultState.Success)
            {
                var result = await _commentService.CreateComment(customerId.Value, request);
                return result.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Client")]
        [HttpDelete("{id}/customer/account")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var customerId = this.GetAuthorizedUserId();
            if (customerId.State == ResultState.Success)
            {
                var result = await _commentService.DeleteComment(customerId.Value, id);
                return result.MakeResponse();
            }
            return customerId.MakeResponse();
        }
    }
}
