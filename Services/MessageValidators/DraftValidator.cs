using ArticleService.Protos;
using ArticleService.Data;
using ArticleService.Services.Domain;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.MessageValidators;

public static class DraftValidator
{
    /// <summary>
    /// Validates the first submission of a draft
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="GazellaValidationException">Thrown if there is at least one issue with the request's data</exception>
    public static void ValidateSubmitDraftRequest(SubmitDraftRequest request)
    {
        var issues = new List<string>();
        
        request.Title = request.Title.Trim();
        ValidateTitle(issues, request.Title);
        
        request.AuthorId = request.AuthorId.Trim();
        ValidateId(issues, request.AuthorId, "AuthorId");
        
        request.CategoryId = request.CategoryId.Trim();
        ValidateId(issues,  request.CategoryId, "CategoryId");
        
        request.CoverUri = request.CoverUri.Trim();
        ValidateCoverUri(issues, request.CoverUri);
        
        request.Summary = request.Summary.Trim();
        ValidateSummary(issues, request.Summary);

        ValidateContent(issues, request.Content);

        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }

    /// <summary>
    /// Validates an update to an existing draft
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="GazellaValidationException">Thrown if there is at least one issue with the request's data</exception>
    public static void ValidateUpdateDraftRequest(UpdateDraftRequest request)
    {
        var issues = new List<string>();
        
        request.DraftId = request.DraftId.Trim();
        ValidateId(issues, request.DraftId,  "DraftId");
        
        request.Title = request.Title.Trim();
        ValidateTitle(issues, request.Title);
        
        request.CoverUri = request.CoverUri.Trim();
        ValidateCoverUri(issues, request.CoverUri);
        
        request.Summary = request.Summary.Trim();
        ValidateSummary(issues, request.Summary);
        
        request.CategoryId = request.CategoryId.Trim();
        ValidateId(issues, request.CategoryId, "CategoryId");
        
        ValidateContent(issues, request.Content);
        
        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }

    public static void ValidatePublishDraftRequest(PublishDraftRequest request)
    {
        var issues = new List<string>();
        
        request.DraftId = request.DraftId.Trim();
        ValidateId(issues, request.DraftId,  "DraftId");
        
        request.Title = request.Title.Trim();
        ValidateTitle(issues, request.Title);
        
        request.CoverUri = request.CoverUri.Trim();
        ValidateCoverUri(issues, request.CoverUri);
        
        request.Summary = request.Summary.Trim();
        ValidateSummary(issues, request.Summary);
        
        request.CategoryId = request.CategoryId.Trim();
        ValidateId(issues, request.CategoryId, "CategoryId");
        
        request.AuthorName = request.AuthorName.Trim();
        ValidateAuthorName(issues, request.AuthorName);

        request.AuthorPfpUri = request.AuthorPfpUri.Trim();
        ValidateAuthorPfpUri(issues, request.AuthorPfpUri);
        
        ValidateContent(issues, request.Content);
        
        if (issues.Count > 0)
        {
            throw new GazellaValidationException(ExceptionUtil.IssueStringify(issues));
        }
    }
    
    private static void ValidateTitle(List<string> issues, string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            issues.Add("Title is required");
        }
        else if (title.Length > EntitySizeConstraints.ArticleTitleMaxLength)
        {
            issues.Add($"Title is too long, max length is {EntitySizeConstraints.ArticleTitleMaxLength}");
        }
    }

    private static void ValidateId(List<string> issues, string id, string idName)
    {
        if (string.IsNullOrEmpty(id))
        {
            issues.Add($"{idName} is required");
        }
        else if (!Guid.TryParse(id, out _))
        {
            issues.Add($"{idName} is not valid UUID");
        }
    }
    
    private static void ValidateAuthorName(List<string> issues, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            issues.Add("Author name is required");
        }
        else if (name.Length > EntitySizeConstraints.AuthorNameMaxLength)
        {
            issues.Add($"Author name is too long, max length is {EntitySizeConstraints.AuthorNameMaxLength}");
        }
    }

    private static void ValidateAuthorPfpUri(List<string> issues, string pfpUri)
    {
        if (pfpUri.Length > EntitySizeConstraints.AuthorProfilePictureUriMaxLength)
        {
            issues.Add($"Author profile picture URI is too long, max length is {EntitySizeConstraints.AuthorProfilePictureUriMaxLength}");
        }
        //todo uri format verification
    }

    private static void ValidateCoverUri(List<string> issues, string coverUri)
    {
        if (coverUri.Length > EntitySizeConstraints.ArticleCoverUriMaxLength)
        {
            issues.Add($"CoverUri is too long, max lenght is {EntitySizeConstraints.ArticleCoverUriMaxLength}");
        }
        //todo uri format verification
    }

    private static void ValidateSummary(List<string> issues, string summary)
    {
        if (summary.Length > EntitySizeConstraints.ArticleSummaryMaxLength)
        {
            issues.Add($"Summary is too long, max lenght is {EntitySizeConstraints.ArticleSummaryMaxLength}");
        }
    }

    private static void ValidateContent(List<string> issues, string content)
    {
        if (!GazellaValidator.IsValidGazellaJson(content))
        {
            issues.Add("Content does not adhere to Editor.js format");
        }
    }
}