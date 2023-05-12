using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace TestApiSalon.Dtos.Other
{
    public class Paging
    {
        public const int MAX_PAGE_SIZE = 50;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be present and greater than 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, MAX_PAGE_SIZE, ErrorMessage = "PageSize must be between 1 and 50")]
        public int PageSize { get; set; } = MAX_PAGE_SIZE;

        [BindNever]
        public int Skip
        {
            get { return PageSize * (PageNumber - 1); }
        }
    }
}
