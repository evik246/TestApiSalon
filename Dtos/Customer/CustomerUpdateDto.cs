﻿using System.ComponentModel.DataAnnotations;
using TestApiSalon.Attributes;

namespace TestApiSalon.Dtos.Customer
{
    [AtLeastOneProperty(ErrorMessage = "At least one property must be specified")]
    public class CustomerUpdateDto
    {
        [StringLength(40, ErrorMessage = "Max length of the name is 40")]
        public string? Name { get; set; }

        [NullableProperty(nameof(IsBirthdayNullable))]
        [Birthday(MinAge = 1, MaxAge = 100, ErrorMessage = "Birthday is invalid")]
        public DateOnly? Birthday { get; set; }

        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Phone number is invalid")]
        [RegularExpression(@"\+[0-9]{12}", ErrorMessage = "Phone number is invalid")]
        public string? Phone { get; set; }

        public bool IsBirthdayNullable { get; set; } = false;
    }
}
