using MediatR;

namespace Backend.Infrastructure.CQRS.Commands
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}