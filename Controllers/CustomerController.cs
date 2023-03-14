using Microsoft.AspNetCore.Mvc;
using TestApiSalon.Dtos;
using TestApiSalon.Exceptions;
using TestApiSalon.Models;
using TestApiSalon.Services;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            var customers = await _customerService.GetAllCustomers();
            return Ok(customers);
        }

        [HttpPost("register")]
        public async Task<ActionResult<Customer>> Register(CustomerRegisterDto request)
        {
            var customer = await _customerService.CreateCustomer(request) ?? throw new ConflictException("This email is already used");
            return Ok(customer);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var token = await _customerService.LoginCustomer(request) ?? throw new UnauthorizedException("Incorrect email or password");
            return Ok(token);
        }
    }
}
