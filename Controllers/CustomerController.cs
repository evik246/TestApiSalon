﻿using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Attributes;
using TestApiSalon.Dtos.Auth;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Dtos.Other;
using TestApiSalon.Extensions;
using TestApiSalon.Services.CustomerService;

namespace TestApiSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Roles("Guest", "Client")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]CustomerRegisterDto request)
        {
            var customer = await _customerService.CreateCustomer(request);
            return customer.MakeResponse();
        }

        [Roles("Client")]
        [HttpGet("account")]
        public async Task<IActionResult> GetCustomer()
        {
            var customerId = this.GetAuthorizedUserId();
            if (customerId.State == ResultState.Success)
            {
                var customer = await _customerService.GetCustomerById(customerId.Value);
                return customer.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerById(id);
            return customer.MakeResponse();
        }

        [Roles("Client")]
        [HttpPut("account")]
        public async Task<IActionResult> ChangeCustomer([FromBody]CustomerUpdateDto request)
        {
            var customerId = this.GetAuthorizedUserId();
            if (customerId.State == ResultState.Success)
            {
                var customer = await _customerService.UpdateCustomer(customerId.Value, request);
                return customer.MakeResponse();
            }
            return customerId.MakeResponse();
        }

        [Roles("Client")]
        [HttpPut("account/reset_password")]
        public async Task<IActionResult> ChangePassword([FromBody] CustomerChangePassword request)
        {
            var customerEmail = this.GetAuthorizedUserEmail();
            if (customerEmail.State == ResultState.Success)
            {
                var result = await _customerService.ResetPassword(customerEmail.Value!, request);
                return result.MakeResponse();
            }
            return customerEmail.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet()]
        public async Task<IActionResult> GetAllCustomers([FromQuery] Paging paging)
        {
            var customers = await _customerService.GetAllCustomers(paging);
            return customers.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("{customerId}/appointment/date/first/manager/account")]
        public async Task<IActionResult> GetFirstCustomerAppointmentDate(int customerId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var firstDate = await _customerService.GetFirstCustomerAppointmentDate(salonId.Value!, customerId);
                return firstDate.MakeResponse();
            }
            return salonId.MakeResponse();
        }

        [Roles("Manager")]
        [HttpGet("{customerId}/appointment/date/last/manager/account")]
        public async Task<IActionResult> GetLastCustomerAppointmentDate(int customerId)
        {
            var salonId = this.GetAuthorizedEmployeeSalonId();
            if (salonId.State == ResultState.Success)
            {
                var lastDate = await _customerService.GetLastCustomerAppointmentDate(salonId.Value!, customerId);
                return lastDate.MakeResponse();
            }
            return salonId.MakeResponse();
        }
    }
}
