using System.Globalization;
using ArticleService.Data.Repositories;
using ArticleService.Protos.Interaction;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Exceptions;
using ArticleService.Services.MessageValidators;
using Grpc.Core;

namespace ArticleService.Services;

public class InteractionService(IInteractionRepository interactionRepository, IArticleRepository articleRepository) : Protos.Interaction.InteractionService.InteractionServiceBase
{
    public override async Task<CommentArticleResponse> CommentArticle(CommentArticleRequest request, ServerCallContext context)
    {
        InteractionValidator.ValidateCommentArticleRequest(request);

        var newComment = new Entities.Comment
        {
            ArticleId = request.ArticleId,
            AuthorId = request.AuthorId,
            AuthorName = request.AuthorName,
            AuthorProfilePictureUri = request.AuthorPfpUri,
            Content = request.Content,
            PostedAt = DateTime.UtcNow
        };
        
        var comment = await interactionRepository.AddCommentAsync(request.ArticleId, newComment);

        if (comment is not Entities.RecentComment)
        {
            throw new GazellaNotFoundException($"No article with id {request.ArticleId} could be found.");
        }

        return new CommentArticleResponse
        {
            Success = true,
            PostedAt = comment.PostedAt.ToString(CultureInfo.InvariantCulture)
        };
    }

    public override async Task<DeleteCommentResponse> DeleteComment(DeleteCommentRequest request, ServerCallContext context)
    {
        InteractionValidator.ValidateDeleteCommentRequest(request);

        var articleExists = await articleRepository.VerifyArticleExists(request.ArticleId);

        if (!articleExists)
        {
            throw new GazellaNotFoundException($"No article with id {request.ArticleId} could be found.");
        }
        
        var deletedComment = await interactionRepository.DeleteCommentAsync(request.ArticleId, request.CommentId);

        if (deletedComment is not Entities.Comment)
        {
            throw new GazellaNotFoundException($"No comment with  id {request.CommentId} could be found.");
        }

        return new DeleteCommentResponse
        {
            Success = true,
            Message = $"Comment {request.CommentId} has been deleted."
        };
    }

    public override async Task<GetCommentsResponse> GetComments(GetCommentsRequest request, ServerCallContext context)
    {
        request.ArticleId = request.ArticleId.Trim();
        if (!Guid.TryParse(request.ArticleId, out _))
        {
            throw new GazellaValidationException("Provided article Id is not a valid UUID");
        }

        InteractionValidator.ValidateGetCommentsRequest(request);
        var index = request.PageIndex - 1;
        var offset = PaginationUtil.GetOffset(index, request.PageSize);

        var result =
            await interactionRepository.GetCommentsAsync(request.ArticleId, offset, request.PageSize);

        var response = new GetCommentsResponse
        {
            TotalComments = result.TotalItems,
            CurrentPage = request.PageIndex,
            PageCount = result.PageCount,
            PageSize = request.PageSize
        };
        response.Comments.AddRange(result.Items.Select(c => new Comment
        {
            Id = c.Id,
            AuthorId =  c.AuthorId,
            AuthorName = c.AuthorName,
            AuthorPfpUri = c.AuthorProfilePictureUri,
            Content = c.Content,
            PostedAt = c.PostedAt.ToString(CultureInfo.InvariantCulture)
        }));
        return response;
    }

    public override async Task<LikeArticleResponse> LikeArticle(LikeArticleRequest request, ServerCallContext context)
    {
        InteractionValidator.ValidateLikeArticleRequest(request);

        var existingArticle = await articleRepository.GetArticleById(request.ArticleId);

        if (existingArticle is not Entities.Article)
        {
            throw new GazellaNotFoundException($"No article with id {request.ArticleId} was found.");
        }
        
        var existingLike = await interactionRepository.GetExistingLikeAsync(request.ArticleId, request.AuthorId);

        if (existingLike is Entities.Like)
        {
            return new LikeArticleResponse
            {
                Message = $"Article {request.ArticleId} has already been liked by user {request.AuthorId}",
                CurrentLikes = existingArticle.Likes
            };
        }

        var result = await interactionRepository.LikeArticleAsync(request.ArticleId, request.AuthorId);

        if (result.Like is not Entities.Like)
        {
            throw new GazellaNotFoundException($"No article with id {request.ArticleId} was found.");
        }

        return new LikeArticleResponse
        {
            Message = $"You have liked article {request.ArticleId} by {request.AuthorId}",
            CurrentLikes = result.CurrentLikes
        };
    }

    public override async Task<RevokeLikeResponse> RevokeLike(RevokeLikeRequest request, ServerCallContext context)
    {
        InteractionValidator.ValidateRevokeLikeRequest(request);
        
        var existingArticle = await articleRepository.GetArticleById(request.ArticleId);

        if (existingArticle is not Entities.Article)
        {
            throw new GazellaNotFoundException($"No article with id {request.ArticleId} was found.");
        }
        
        var foundLike = await interactionRepository.GetExistingLikeAsync(request.ArticleId, request.AuthorId);

        if (foundLike is not Entities.Like existingLike)
        {
            return new RevokeLikeResponse
            {
                Message = $"User {request.AuthorId} has not liked article {request.ArticleId}",
                CurrentLikes = existingArticle.Likes
            };
        }

        var result = await interactionRepository.RevokeLikeAsync(request.ArticleId, existingLike.Id);

        if (result.Like is Entities.Like)
        {
            return new RevokeLikeResponse
            {
                Message = $"Like of {request.AuthorId} has been revoked from {request.ArticleId}",
                CurrentLikes = result.CurrentLikes
            };
        }
        throw new GazellaNotFoundException(
            $"Like ${existingLike.Id} was not found. Either the like has already been removed, " +
            $"or article {request.ArticleId} no longer exists");
    }
}