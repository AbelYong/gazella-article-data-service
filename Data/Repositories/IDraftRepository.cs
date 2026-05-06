using ArticleService.Entities;

namespace ArticleService.Data.Repositories;

public interface IDraftRepository
{
    Task<string> SaveDraft(Article draft);
}