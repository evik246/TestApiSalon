﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TestApiSalon.Models;

namespace TestApiSalon.Dtos.Employee
{
    public class EmployeeCreateDto
    {
        [StringLength(40, ErrorMessage = "Max length of the name is 40")]
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [StringLength(40, ErrorMessage = "Max length of the name is 40")]
        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password should be greater than or equal 8 characters")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required EmployeeRole Role { get; set; }

        public int? SalonId { get; set; }

        public string? Specialization { get; set; }
    }
}
