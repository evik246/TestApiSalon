using TestApiSalon.Dtos.Comment;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Models;

namespace TestApiSalon.Services.CommentService
{
    public interface ICommentService
    {
        Task<Result<IEnumerable<Comment>>> GetAllComments(Paging paging, int? salonId = null);
        Task<Result<string>> CreateComment(CommentCreateDto request);
    }
}
