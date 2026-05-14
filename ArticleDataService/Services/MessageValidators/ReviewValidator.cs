using ArticleService.Data;
using ArticleService.Protos;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.MessageValidators;

public static class ReviewValidator
{
    /// <summary>
    /// Validates if the request for pending articles has a valid page index and page size.
    /// And returns the calculated offset. Assumes a page index starting at zero
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="GazellaValidationException">Thrown if index, size or offset validation fail</exception>
    public static void ValidateArticlesPendingRequest(GetArticlesPendingReviewRequest request)
    {
        PaginationUtil.ValidatePageIndex(request.PageIndex);
        PaginationUtil.ValidatePageSize(request.PageSize);
        PaginationUtil.ValidatePageOffset(request.PageIndex,  request.PageSize);
    }

    public static void ValidateApproveArticleRequest(ApproveArticleRequest request)
    {
        var issues = new List<string>();
        
        request.ArticleId = request.ArticleId.Trim();
        GeneralValidator.ValidateId(issues, request.ArticleId, "ArticleId");
        
        request.ReviewedById = request.ReviewedById.Trim();
        GeneralValidator.ValidateId(issues, request.ReviewedById, "ReviewedById");
        
        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }

    public static void ValidateRejectArticleRequest(RejectArticleRequest request)
    {
        var issues = new List<string>();
        
        request.ArticleId = request.ArticleId.Trim();
        GeneralValidator.ValidateId(issues, request.ArticleId, "ArticleId");
        
        request.ReviewedById = request.ReviewedById.Trim();
        GeneralValidator.ValidateId(issues, request.ReviewedById, "ReviewedById");
        
        request.RejectionReason =  request.RejectionReason.Trim();
        if (string.IsNullOrEmpty(request.RejectionReason))
        {
            issues.Add("Rejection reason is required");
        }
        else if (request.RejectionReason.Length > EntitySizeConstraints.ReviewRejectionReasonMaxLength)
        {
            issues.Add($"Rejection reason is too long, max length is {EntitySizeConstraints.ReviewRejectionReasonMaxLength} characters");
        }
        
        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }
}