using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Backend.Users.Commands.Users.CreateUser;
using Backend.Users.Queries.Users.FindUserById;
using Backend.Users.Queries.Users.FindUserByLoginPassword;
using Backend.Web.Users.ViewModels.Users.Requests;
using Backend.Web.Users.ViewModels.Users.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Web.Users.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UsersController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public async Task<UserViewModel> CreateUserAsync(
            [FromBody] CreateUserRequestModel model,
            CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateUserCommand>(model);
            var userId = await _mediator.Send(command, cancellationToken);
            var query = _mapper.Map<FindUserByIdQuery>(userId);
            var user = await _mediator.Send(query, cancellationToken);
            var viewModel = _mapper.Map<UserViewModel>(user);
            return viewModel;
        }

        [HttpGet("{userId:long:min(1)}")]
        public async Task<IActionResult> FindUserByIdAsync(
            [FromRoute] long userId,
            CancellationToken cancellationToken)
        {
            var query = _mapper.Map<FindUserByIdQuery>(userId);
            var user = await _mediator.Send(query, cancellationToken);
            if (user == null)
                return NotFound();
            var viewModel = _mapper.Map<UserViewModel>(user);
            return Ok(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FindUserByLoginPasswordAsync(
            [FromQuery] FindUserByLoginPasswordRequestModel model,
            CancellationToken cancellationToken)
        {
            var query = _mapper.Map<FindUserByLoginPasswordQuery>(model);
            var user = await _mediator.Send(query, cancellationToken);
            if (user == null)
                return NotFound();
            var viewModel = _mapper.Map<UserViewModel>(user);
            return Ok(viewModel);
        }
    }
}