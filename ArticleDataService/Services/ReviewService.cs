using ArticleService.Data.Repositories;
using ArticleService.Entities;
using ArticleService.Protos.Review;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Exceptions;
using ArticleService.Services.MessageValidators;
using Grpc.Core;

namespace ArticleService.Services;

public class ReviewService(IReviewRepository reviewRepository) : Protos.Review.ReviewService.ReviewServiceBase
{
    public override async Task<GetArticlesPendingReviewResponse> GetArticlesPendingReview(GetArticlesPendingReviewRequest request, ServerCallContext context)
    {
        ReviewValidator.ValidateArticlesPendingRequest(request);
        var index = request.PageIndex - 1;
        var offset = PaginationUtil.GetOffset(index, request.PageSize);

        var articlesPendingReviewPage = await reviewRepository.GetArticlesPendingReview(offset, request.PageSize);

        var response = new GetArticlesPendingReviewResponse
        {
            TotalPending = articlesPendingReviewPage.TotalItems,
            CurrentPage = request.PageIndex,
            PageCount = articlesPendingReviewPage.PageCount,
            PageSize = request.PageSize,
        };
        response.ArticlesPending.AddRange(articlesPendingReviewPage.Items.Select(a => new ArticlePendingReview
        {
            ArticleId = a.Id,
            Title = a.Title,
            AuthorName = a.AuthorName,
            Category = a.Category,
            SubmittedAt = a.LastUpdatedAt != null ? a.LastUpdatedAt?.ToString("O") : ""
        }));
        return response;
    }

    public override async Task<ApproveArticleResponse> ApproveArticle(ApproveArticleRequest request, ServerCallContext context)
    {
        ReviewValidator.ValidateApproveArticleRequest(request);
        
        var article = await reviewRepository.GetArticlePendingReview(request.ArticleId);

        if (article is not Article toReview)
        {
            throw new GazellaNotFoundException($"No article pending review with Id: {request.ArticleId} could be found.");
        }

        toReview.ReviewMetadata = new Review
        {
            IsApproved = true,
            ReviewedById = request.ReviewedById,
            ReviewedAt = DateTimeOffset.UtcNow
        };

        var result = await reviewRepository.UpdateArticleReview(toReview, ArticleStatus.Published);

        if (!result.IsSuccess)
        {
            throw new GazellaConcurrencyException(
                $"Article {request.ArticleId} is no longer under review. Its current state is {result.ConcurrencyTokenField}");
        }

        return new ApproveArticleResponse
        {
            ArticleStatus = nameof(ArticleStatus.Published),
            Message = $"Article {request.ArticleId} has been approved for publication"
        };
    }

    public override async Task<RejectArticleResponse> RejectArticle(RejectArticleRequest request, ServerCallContext context)
    {
        ReviewValidator.ValidateRejectArticleRequest(request);
        
        var article = await reviewRepository.GetArticlePendingReview(request.ArticleId);

        if (article is not Article toReview)
        {
            throw new GazellaNotFoundException($"No article pending review with Id: {request.ArticleId} could be found.");
        }
        
        toReview.ReviewMetadata = new Review
        {
            IsApproved = false,
            RejectionReason = request.RejectionReason,
            ReviewedById = request.ReviewedById,
            ReviewedAt = DateTimeOffset.UtcNow
        };
        
        var result = await reviewRepository.UpdateArticleReview(toReview, ArticleStatus.Rejected);

        if (!result.IsSuccess)
        {
            throw new GazellaConcurrencyException(
                $"Article {request.ArticleId} is no longer under review. Its current state is {result.ConcurrencyTokenField}");
        }

        return new RejectArticleResponse
        {
            ArticleStatus = nameof(ArticleStatus.Rejected),
            Message = $"Publication of article {request.ArticleId} has been rejected"
        };
    }
}