using System.Globalization;
using ArticleService.Data.Repositories;
using ArticleService.Protos.Article;
using ArticleService.Services.Exceptions;
using Grpc.Core;

namespace ArticleService.Services;

public class ArticleService(IArticleRepository articleRepository, ICategoryRepository categoryRepository) 
    : Protos.Article.ArticleService.ArticleServiceBase
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
        
        var articles = await articleRepository.GetArticlesByAuthorId(request.Id);

        var response = new GetMyArticlesResponse();
        
        response.MyArticles.AddRange(articles.Select(a => new MyArticle
        {
            ArticleId =  a.Id,
            Title = a.Title,
            Status = a.Status.ToString(),
            Category = a.Category,
            PublishedAt = a.PublishedAt.ToString(),
            Likes = a.Likes,
            Comments = a.CommentsCount
        }));

        return response;
    }

    public override async Task<GetArticleResponse> GetArticle(GetArticleRequest request, ServerCallContext context)
    {
        request.Id = request.Id.Trim();
        if (!Guid.TryParse(request.Id, out _))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Provided Id is not a valid UUID"));
        }

        var article = await articleRepository.GetArticleById(request.Id);

        if (article is not Entities.Article foundArticle)
        {
            throw new GazellaNotFoundException($"No article matching id: {request.Id} could be found");
        }
        
        var response = new GetArticleResponse
        {
            Id = foundArticle.Id,
            Title = foundArticle.Title,
            CoverUri = foundArticle.CoverUri,
            Summary = foundArticle.Summary,
            Category = foundArticle.Category,
            PublishedAt = foundArticle.PublishedAt.ToString(),
            LastUpdatedAt = foundArticle.LastUpdatedAt.ToString(),
            Status = foundArticle.Status.ToString(),
            Content = foundArticle.Content,
            AuthorId = foundArticle.AuthorId,
            AuthorName = foundArticle.AuthorName,
            AuthorPfpUri = foundArticle.AuthorProfilePictureUri,
            LikesCount = foundArticle.Likes,
            CommentsCount = foundArticle.CommentsCount,
        };
        response.RecentComments.AddRange(article.Comments.Select(c => new RecentComment
        {
            Id = c.Id,
            AuthorId = c.AuthorId,
            AuthorName = c.AuthorName,
            AuthorPfpUri = c.AuthorProfilePictureUri,
            Content = c.Content,
            PostedAt = c.PostedAt.ToString(CultureInfo.InvariantCulture)
        }));
        return response;
    }
}
