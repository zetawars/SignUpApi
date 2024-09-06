using Application.DTOs;
using Application.Features.Users.Commands;
using Application.Features.Users.Querries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SignUpApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            await _mediator.Send(new SignupCommand { Request = request });
            return Ok();
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            var response = await _mediator.Send(new AuthenticateCommand { Request = request });
            return Ok(response);
        }

        [HttpPost("auth/balance")]
        public async Task<IActionResult> GetBalance([FromBody] GetBalanceQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }


}
