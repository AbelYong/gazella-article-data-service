using ArticleService.Protos;
using ArticleService.Data;
using ArticleService.Services.Domain;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.MessageValidators;

public static class DraftValidator
{
    /// <summary>
    /// Validates a
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="GazellaDomainException">Thrown if there is at least one issue with the request's data</exception>
    public static void ValidateSubmitDraftRequest(SubmitDraftRequest request)
    {
        List<string> issues = new List<string>();
        
        request.Title = request.Title.Trim();
        if (string.IsNullOrEmpty(request.Title))
        {
            issues.Add("Title is required");
        }
        else if (request.Title.Length > EntitySizeConstraints.ArticleTitleMaxLength)
        {
            issues.Add("Title is too long");
        }
        
        request.AuthorId = request.AuthorId.Trim();
        if (string.IsNullOrEmpty(request.AuthorId))
        {
            issues.Add("AuthorId is required");
        }
        else if (!Guid.TryParse(request.AuthorId, out _))
        {
            issues.Add("AuthorId is not valid UUID");
        }
        
        request.CategoryId = request.CategoryId.Trim();
        if (string.IsNullOrEmpty(request.CategoryId))
        {
            issues.Add("CategoryId is required");
        }
        else if (!Guid.TryParse(request.CategoryId, out _))
        {
            issues.Add("CategoryId is not valid UUID");
        }
        
        request.CoverUri = request.CoverUri.Trim();
        if (request.CoverUri.Length > EntitySizeConstraints.ArticleCoverUriMaxLength)
        {
            issues.Add("CoverUri is too long");
        }
        //todo uri format verification
        
        request.Summary = request.Summary.Trim();
        if (request.Summary.Length > EntitySizeConstraints.ArticleSummaryMaxLength)
        {
            issues.Add("Summary is too long");
        }

        if (!GazellaValidator.IsValidGazellaJson(request.Content))
        {
            issues.Add("Content does not adhere to Editor.js format");
        }

        if (issues.Count > 0)
        {
            throw new GazellaDomainException(ExceptionUtil.IssueStringify(issues));
        }
    }
}