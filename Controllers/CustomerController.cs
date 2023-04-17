using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
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
            var customer = await _customerService.CreateCustomer(request) 
                ?? throw new ConflictException("This email or phone number is already used");
            return Ok(customer);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerById(id) 
                ?? throw new NotFoundException("Client is not found");
            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeCustomer(int id, [FromBody]CustomerUpdateDto request)
        {
            var customer = await _customerService.UpdateCustomer(id, request) 
                ?? throw new NotFoundException("Client is not found");
            return Ok(customer);
        }
    }
}
