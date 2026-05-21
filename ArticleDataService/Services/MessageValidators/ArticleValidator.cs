using System.Globalization;
using ArticleService.Protos.Article;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Domain;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.MessageValidators;

public static class ArticleValidator
{
    public static ArticleSearch ValidateSearchArticlesRequest(SearchArticlesRequest request)
    {
        var issues = new List<string>();
        var articleSearch = new ArticleSearch();
        
        PaginationUtil.ValidatePageIndex(request.PageIndex);
        PaginationUtil.ValidatePageSize(request.PageSize);
        PaginationUtil.ValidatePageOffset(request.PageIndex, request.PageSize);
        
        if (request.Filter == null)
        {
            return articleSearch; 
        }
        
        articleSearch.Title = request.Filter.Title.ToLower().Trim();
        articleSearch.Category = request.Filter.Category.ToLower().Trim();
        articleSearch.AuthorName = request.Filter.AuthorName.ToLower().Trim();

        if (!string.IsNullOrWhiteSpace(request.Filter.PublishedAfter))
        {
            if (DateTime.TryParse(request.Filter.PublishedAfter, CultureInfo.InvariantCulture, 
                    DateTimeStyles.RoundtripKind, out var parsedDate))
            {
                articleSearch.PublishedAfter = parsedDate;
            }
            else
            {
                issues.Add($"PublishedAfter is not a valid ISO 8601 Date, received {request.Filter.PublishedAfter}");
            }
        }
        
        articleSearch.SortBy = request.Filter.SortBy.ToLower().Trim();
        List<string> sortFilers = ["published_at", "views", "comments", "likes"];

        if (!sortFilers.Contains(articleSearch.SortBy) && articleSearch.SortBy != string.Empty)
        {
            issues.Add($"{articleSearch.SortBy} is not an allowed sorting filter, " +
                       $"allowed filters are: {string.Join( ", ", sortFilers)}");
        }

        return issues.Count == 0 ? articleSearch : 
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
    }
}