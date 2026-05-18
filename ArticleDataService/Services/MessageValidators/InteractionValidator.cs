using ArticleService.Data;
using ArticleService.Protos.Interaction;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.MessageValidators;

public static class InteractionValidator
{
    private const string ArticleId = "ArticleId";
    private const string AuthorId = "AuthorId";
    
    public static void ValidateCommentArticleRequest(CommentArticleRequest request)
    {
        var issues = new List<string>();
        request.ArticleId = request.ArticleId.Trim();
        GeneralValidator.ValidateId(issues, request.ArticleId, ArticleId);
        
        request.AuthorId = request.AuthorId.Trim();
        GeneralValidator.ValidateId(issues, request.AuthorId, AuthorId);
        
        request.AuthorName = request.AuthorName.Trim();
        GeneralValidator.ValidateAuthorName(issues, request.AuthorName);
        
        request.AuthorPfpUri = request.AuthorPfpUri.Trim();
        GeneralValidator.ValidateAuthorPfpUri(issues, request.AuthorPfpUri);

        request.Content = request.Content.Trim();
        if (string.IsNullOrEmpty(request.Content))
        {
            issues.Add("A comment's content cannot be empty");
        }
        else if (request.Content.Length > EntitySizeConstraints.CommentContentMaxLength)
        {
            issues.Add($"A comment's content cannot be longer than {EntitySizeConstraints.CommentContentMaxLength}");
        }

        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }

    public static void ValidateDeleteCommentRequest(DeleteCommentRequest request)
    {
        var issues = new List<string>();
        request.ArticleId = request.ArticleId.Trim();
        GeneralValidator.ValidateId(issues, request.ArticleId, ArticleId);

        request.CommentId = request.CommentId.Trim();
        GeneralValidator.ValidateId(issues, request.CommentId, "CommentId");
        
        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }
    
    public static void ValidateGetCommentsRequest(GetCommentsRequest request)
    {
        PaginationUtil.ValidatePageIndex(request.PageIndex);
        PaginationUtil.ValidatePageSize(request.PageSize);
        PaginationUtil.ValidatePageOffset(request.PageIndex, request.PageSize);
    }

    public static void ValidateLikeArticleRequest(LikeArticleRequest request)
    {
        var issues = new List<string>();
        request.ArticleId = request.ArticleId.Trim();
        GeneralValidator.ValidateId(issues, request.ArticleId, ArticleId);
        
        request.AuthorId = request.AuthorId.Trim();
        GeneralValidator.ValidateId(issues, request.AuthorId, AuthorId);

        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }
    
    public static void ValidateRevokeLikeRequest(RevokeLikeRequest request)
    {
        var issues = new List<string>();
        request.ArticleId = request.ArticleId.Trim();
        GeneralValidator.ValidateId(issues, request.ArticleId, ArticleId);
        
        request.AuthorId = request.AuthorId.Trim();
        GeneralValidator.ValidateId(issues, request.AuthorId, AuthorId);

        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }
}