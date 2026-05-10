using ArticleService.Data.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ArticleService.Services.Exceptions;

public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
    private const string GazellaError = "x-gazella-error";
    private const string InvalidArgument = "invalid_argument";
    private const string DbUnavailable = "db_unavailable";

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (GazellaDomainException ex)
        {
            var metadata = new Metadata { { GazellaError, InvalidArgument } };
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Issues), metadata);
        }
        catch (GazellaDbException)
        {
            var metadata = new Metadata { { GazellaError, DbUnavailable } };
            throw new RpcException(
                new Status(
                    StatusCode.Unavailable, 
                    "The database is not available, it took to long to respond or another internal issue"),
                metadata
                );
        }
        catch (Exception ex)
        {
            // El logger ahora captura el nombre del método gRPC que falló a través del context
            logger.LogError(ex, "Unexpected exception while processing {Method}: {Ex}", context.Method, ex.Message);
            throw new RpcException(new Status(StatusCode.Internal, "Internal Server Error"));
        }
    }
}