using Grpc.Core;
using ArticleService.Data.Repositories;
using ArticleService.Entities;
using ArticleService.Protos;
using ArticleService.Services.Domain;
using ArticleService.Services.Exceptions;
using ArticleService.Services.MessageValidators;

namespace ArticleService.Services;

public class DraftService(IDraftRepository draftRepository, ICategoryRepository categoryRepository) 
    : Protos.DraftService.DraftServiceBase
{
    public override async Task<SubmitDraftResponse> SubmitDraft(SubmitDraftRequest request, ServerCallContext context)
    {
        DraftValidator.ValidateSubmitDraftRequest(request);
        var category = await GazellaValidator.VerifyExistingCategory(categoryRepository, request.CategoryId);

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

    public override async Task<UpdateDraftResponse> UpdateDraft(UpdateDraftRequest request, ServerCallContext context)
    {
        DraftValidator.ValidateUpdateDraftRequest(request);
        var category = await GazellaValidator.VerifyExistingCategory(categoryRepository, request.CategoryId);
        var existingDraft = await draftRepository.GetExistingDraft(request.DraftId);

        if (existingDraft.Status == ArticleStatus.UnderReview)
        {
            throw new GazellaInvalidOperationException("Drafts under review cannot be updated");
        }

        if (existingDraft is not Article updatedDraft)
        {
            throw new GazellaNotFoundException($"No draft was found for id: {request.DraftId}");
        }
        
        updatedDraft.Title = request.Title;
        updatedDraft.CoverUri = request.CoverUri;
        updatedDraft.Summary = request.Summary;
        updatedDraft.Category = category.Name;
        updatedDraft.Content = request.Content;
            
        await draftRepository.UpdateDraft(updatedDraft);

        return new UpdateDraftResponse
        {
            IsSuccess = true,
            Message = "Draft successfully updated"
        };
    }

    public override async Task<PublishDraftResponse> PublishDraft(PublishDraftRequest request, ServerCallContext context)
    {
        DraftValidator.ValidatePublishDraftRequest(request);
        var category = await GazellaValidator.VerifyExistingCategory(categoryRepository, request.CategoryId);
        var existingDraft = await draftRepository.GetExistingDraft(request.DraftId);
            
        if (existingDraft is not Article toPublish)
        {
            throw new GazellaNotFoundException($"No draft was found for id: {request.DraftId}");
        }

        toPublish.Title = request.Title;
        toPublish.CoverUri = request.CoverUri;
        toPublish.Summary = request.Summary;
        toPublish.Category = category.Name;
        toPublish.Author.Name = request.AuthorName;
        toPublish.Author.ProfilePictureUri = request.AuthorPfpUri;
        toPublish.Content = request.Content;

        var status = await draftRepository.SaveDraftPublication(toPublish);

        return new PublishDraftResponse
        {
            ArticleStatus = status,
            Message = "Draft successfully published"
        };
    }
}
