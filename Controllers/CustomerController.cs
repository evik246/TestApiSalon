using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos.Customer;
using TestApiSalon.Exceptions;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]CustomerRegisterDto request)
        {
            var customer = await _customerService.CreateCustomer(request);
            return customer.MakeResponse();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerById(id);
            return customer.MakeResponse();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeCustomer(int id, [FromBody]CustomerUpdateDto request)
        {
            var customer = await _customerService.UpdateCustomer(id, request);
            return customer.MakeResponse();
        }
    }
}
