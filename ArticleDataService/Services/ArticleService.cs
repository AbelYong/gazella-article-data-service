using ArticleService.Data.Repositories;
using ArticleService.Protos.Article;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Exceptions;
using ArticleService.Services.MessageValidators;
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
            PublishedAt = a.PublishedAt != null ? a.PublishedAt?.ToString("O") : "",
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
            PublishedAt = foundArticle.PublishedAt != null ? foundArticle.PublishedAt?.ToString("O") : "",
            LastUpdatedAt = foundArticle.LastUpdatedAt != null ? foundArticle.LastUpdatedAt?.ToString("O") : "",
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
            PostedAt = c.PostedAt.ToString("O")
        }));
        return response;
    }

    public override async Task<SearchArticlesResponse> SearchArticles(SearchArticlesRequest request, ServerCallContext context)
    {
        var search = ArticleValidator.ValidateSearchArticlesRequest(request);
        var index = request.PageIndex - 1;
        var offset = PaginationUtil.GetOffset(index, request.PageSize);

        var result = await articleRepository.SearchArticlesAsync(search, offset, request.PageSize);

        var response = new SearchArticlesResponse();
        response.Entries.AddRange(result.Items.Select(a => new ArticleEntry
        {
            Id = a.Id,
            Title = a.Title,
            AuthorId = a.AuthorId,
            AuthorName = a.AuthorName,
            CategoryName = a.Category,
            Summary = a.Summary,
            PublishedAt = a.PublishedAt != null ? a.PublishedAt?.ToString("O") : "",
            LastUpdatedAt = a.LastUpdatedAt != null ? a.LastUpdatedAt?.ToString("O") : "",
        }));
        response.TotalEntries = result.TotalItems;
        response.CurrentPage = request.PageIndex;
        response.PageSize = request.PageSize;
        response.PageCount = result.PageCount;
        return response;
    }
}
