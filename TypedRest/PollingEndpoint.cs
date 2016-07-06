﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace TypedRest
{
    /// <summary>
    /// REST endpoint that represents an entity that can be polled for state changes.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity the endpoint represents.</typeparam>
    public class PollingEndpoint<TEntity> : ElementEndpoint<TEntity>, IPollingEndpoint<TEntity>
    {
        private readonly Predicate<TEntity> _endCondition;

        /// <summary>
        /// Creates a new polling endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s.</param>
        /// <param name="endCondition">A check to determine whether the entity has reached its final state an no further polling is required.</param>
        public PollingEndpoint(IEndpoint referrer, Uri relativeUri, Predicate<TEntity> endCondition = null)
            : base(referrer, relativeUri)
        {
            _endCondition = endCondition;
        }

        /// <summary>
        /// Creates a new polling endpoint.
        /// </summary>
        /// <param name="referrer">The endpoint used to navigate to this one.</param>
        /// <param name="relativeUri">The URI of this endpoint relative to the <paramref name="referrer"/>'s. Prefix <c>./</c> to append a trailing slash to the <paramref name="referrer"/> URI if missing.</param>
        /// <param name="endCondition">A check to determine whether the entity has reached its final state an no further polling is required.</param>
        public PollingEndpoint(IEndpoint referrer, string relativeUri, Predicate<TEntity> endCondition = null)
            : base(referrer, relativeUri)
        {
            _endCondition = endCondition;
        }

        public IObservable<TEntity> GetStream(TimeSpan pollingInterval)
        {
            return Observable.Create<TEntity>(async (observer, cancellationToken) =>
            {
                TEntity previousEntity;
                try
                {
                    previousEntity = await ReadAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                    return;
                }
                observer.OnNext(previousEntity);

                while (_endCondition == null || !_endCondition(previousEntity))
                {
                    try
                    {
                        await Task.Delay(pollingInterval, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }

                    TEntity newEntity;
                    try
                    {
                        newEntity = await ReadAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }
                    if (!newEntity.Equals(previousEntity))
                        observer.OnNext(newEntity);

                    previousEntity = newEntity;
                }
                observer.OnCompleted();
            });
        }
    }
}