using MediatR;

namespace OrderService.Application.Common.Models;

/// <summary>
/// Base class for all queries in the application.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public abstract record Query<TResult> : IRequest<TResult>;

/// <summary>
/// Base class for all commands in the application.
/// </summary>
public abstract record Command : IRequest;

/// <summary>
/// Base class for all commands that return a result.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public abstract record Command<TResult> : IRequest<TResult>;