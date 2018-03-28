using MediatR;

namespace Backend.Infrastructure.CQRS.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}