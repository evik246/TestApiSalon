using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAllComments([FromQuery] Paging paging)
        {
            var comments = await _commentService.GetAllComments(paging);
            return comments.MakeResponse();
        }

        [HttpGet("salon/{id}")]
        public async Task<IActionResult> GetAllCommentsInSalon(int id, [FromQuery] Paging paging)
        {
            var comments = await _commentService.GetAllComments(paging, id);
            return comments.MakeResponse();
        }
    }
}
