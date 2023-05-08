using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos
{
    public class Paging
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be present and greater than 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be present and greater than 0")]
        public int PageSize { get; set; } = 100;

        [BindNever]
        public int Skip 
        { 
            get { return PageSize * (PageNumber - 1); } 
        }
    }
}
