using MediatR;

namespace OrderService.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for a query handler that returns a result.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IRequest<TResult>
{
}

/// <summary>
/// Defines the contract for a command handler.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : IRequest
{
}

/// <summary>
/// Defines the contract for a command handler that returns a result.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : IRequest<TResult>
{
}