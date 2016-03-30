namespace Bolt.RequestBus.Handlers
{
    public abstract class RequestHandlerBase<TRequest, TResult> : IRequestHandler<TRequest, TResult>
        where TRequest : IRequest
    {
        public IResponse<TResult> Handle(TRequest msg)
        {
            var result = Process(msg);

            return Response.Succeed(result);
        }

        protected abstract TResult Process(TRequest msg);
    }


    public abstract class RequestHandlerBase<TRequest> : IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        public IResponse Handle(TRequest msg)
        {
            Process(msg);

            return Response.Succeed();
        }

        protected abstract void Process(TRequest msg);
    }
}