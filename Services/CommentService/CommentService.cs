using Dapper;
using System.Data;
using System.Text;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Dtos.Salon;
using TestApiSalon.Models;
using TestApiSalon.Services.ConnectionService;

namespace TestApiSalon.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IDbConnectionService _connectionService;

        public CommentService(IDbConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<Result<IEnumerable<Comment>>> GetAllComments(Paging paging, int? salonId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Skip", paging.Skip, DbType.Int32);
            parameters.Add("Take", paging.PageSize, DbType.Int32);

            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT c.id, c.published_date, c.review, c.rating, ");
            builder.Append("cu.id, cu.name, s.id, s.address ");
            builder.Append("FROM Comment c ");
            builder.Append("JOIN Customer cu ON c.customer_id = cu.id ");
            builder.Append("JOIN Salon s ON c.salon_id = s.id ");

            if (salonId != null)
            {
                builder.Append("WHERE c.salon_id = @SalonId ");
                parameters.Add("SalonId", salonId, DbType.Int32);
            }

            builder.Append("ORDER BY c.published_date DESC ");
            builder.Append("OFFSET @Skip LIMIT @Take;");

            using (var connection =  _connectionService.CreateConnection())
            {
                var comments = await connection.QueryAsync(
                    builder.ToString(), (Comment comment, CustomerDto customer, SalonDto salon) =>
                    {
                        comment.Salon = salon;
                        comment.Customer = customer;
                        comment.SalonId = salon.Id;
                        comment.CustomerId = customer.Id;
                        return comment;
                    }, param: parameters
                );
                return new Result<IEnumerable<Comment>>(comments);
            }
        }
    }
}
