using ArticleService.Data.Repositories;
using ArticleService.Entities;
using ArticleService.Protos;
using ArticleService.Services.Exceptions;
using Grpc.Core;
using Category = ArticleService.Protos.Category;
using Comment = ArticleService.Protos.Comment;

namespace ArticleService.Services;

public class ArticleService(IArticleRepository articleRepository, ICategoryRepository categoryRepository) 
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

        if (article is Article foundArticle)
        {
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
            response.RecentComments.AddRange(article.Comments.Select(c => new Comment
            {
                AuthorId = c.AuthorId,
                AuthorName = c.AuthorName,
                AuthorPfpUri = c.AuthorProfilePictureUri,
                Content = c.Content,
                PostedAt = c.PostedAt.ToString()
            }));
            return response;
        }
        else
        {
            throw new GazellaNotFoundException($"No article matching id: {request.Id} could be found");
        }
    }
}
