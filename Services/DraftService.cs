using Grpc.Core;
using ArticleService.Data.Exceptions;
using ArticleService.Data.Repositories;
using ArticleService.Entities;
using ArticleService.Protos;
using ArticleService.Services.Domain;
using ArticleService.Services.Exceptions;
using ArticleService.Services.MessageValidators;

namespace ArticleService.Services;

public class DraftService(IDraftRepository draftRepository, ICategoryRepository categoryRepository, ILogger<DraftService> logger) 
    : Protos.DraftService.DraftServiceBase
{
    public override async Task<SubmitDraftResponse> SubmitDraft(SubmitDraftRequest request, ServerCallContext context)
    {
        try
        {
            DraftValidator.ValidateSubmitDraftRequest(request);
            var category =
                await GazellaValidator.VerifyExistingCategory(categoryRepository, request.CategoryId);

            var draft = new Article
            {
                Title = request.Title,
                CoverUri = request.CoverUri,
                Summary = request.Summary,
                Category = category.Name,
                Author = new Author
                {
                    Id = request.AuthorId
                },
                Content = request.Content
            };
            
            var draftId = await draftRepository.SaveDraft(draft);

            return new SubmitDraftResponse
            {
                ArticleId = draftId,
                Message = "Draft submitted successfully"
            };
        }
        catch (GazellaDomainException ex)
        {
            var metadata = new Metadata { {"x-gazella-error", "invalid_argument"} };
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Issues), metadata);
        }
        catch (GazellaDbException)
        {
            var metadata = new Metadata { {"x-gazella-error", "db_unavailable"} };
            throw new RpcException(new Status(
                StatusCode.Unavailable, 
                "The database is not available, it took to long to respond or another internal issue"),
                metadata);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while processing draft submission: {Ex}", ex.Message);
            throw new RpcException(new Status(StatusCode.Internal, "Internal Server Error"));
        }
    }
}
