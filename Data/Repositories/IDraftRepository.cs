using ArticleService.Entities;
using ArticleService.Entities.Interfaces;

namespace ArticleService.Data.Repositories;

public interface IDraftRepository
{
    Task<IArticle> GetExistingDraft(string id); 
    Task<string> SaveDraft(Article draft);
    Task UpdateDraft(Article draft);
    Task<string> SaveDraftPublication(Article draft);
}