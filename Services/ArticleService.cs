using ArticleService.Data.Exceptions;
using ArticleService.Data.Repositories;
using ArticleService.Protos;
using Grpc.Core;

namespace ArticleService.Services;

public class ArticleService(ICategoryRepository categoryRepository, ILogger<ArticleService> logger) 
    : Protos.ArticleService.ArticleServiceBase
{
    public override async Task<GetCategoriesResponse> GetCategories(GetCategoriesRequest request, ServerCallContext context)
    {
        try
        {
            var categories = await categoryRepository.GetCategories();

            var response = new GetCategoriesResponse();
            response.Categories.AddRange(categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            }));
            
            return response;
        }
        catch (GazellaDbException)
        {
            var metadata = new Metadata { {"x-gazella-error", "db_unavailable"} };
            throw new RpcException(new Status(
                    StatusCode.Unavailable, 
                    "The database is not available, it took to long to respond or another internal issue"),
                metadata);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while processing get categories request: {Ex}", ex.Message);
            throw new RpcException(new Status(StatusCode.Internal, "Internal Server Error"));
        }
    }
}
