using MassTransitTest.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitTest.Api.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("test")]
        public async Task<RegistrationResponse> Get(RegistrationRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}