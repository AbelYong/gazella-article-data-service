using ArticleService.Data.Repositories;
using ArticleService.Protos;
using Grpc.Core;

namespace ArticleService.Services;

public class ArticleService(IArticlesRepository articlesRepository, ICategoryRepository categoryRepository) 
    : Protos.ArticleService.ArticleServiceBase
{
    public override async Task<GetCategoriesResponse> GetCategories(GetCategoriesRequest request, ServerCallContext context)
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

    public override async Task<GetMyArticlesResponse> GetMyArticles(GetMyArticlesRequest request, ServerCallContext context)
    {
        request.Id = request.Id.Trim();
        if (!Guid.TryParse(request.Id, out _))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Provided Id is not a valid UUID"));
        }
        
        var articles = await articlesRepository.GetArticlesByAuthorId(request.Id);

        var response = new GetMyArticlesResponse();
        
        response.MyArticles.AddRange(articles.Select(c => new MyArticle
        {
            ArticleId =  c.Id,
            Title = c.Title,
            Status = c.Status.ToString(),
            Category = c.Category,
            PublishedAt = c.PublishedAt.ToString(),
            Likes = c.Likes,
            Comments = c.CommentsCount
        }));

        return response;
    }
}
