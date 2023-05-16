﻿using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateDto request)
        {
            var result = await _commentService.CreateComment(request);
            return result.MakeResponse();
        }

        [Roles("Client")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var result = await _commentService.DeleteComment(id);
            return result.MakeResponse();
        }
    }
}
