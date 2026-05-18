using ArticleService.Entities.Interfaces;

namespace ArticleService.Services.DataPackages;

public class UpdateLikeResult
{
    public required ILike Like { get; set; }
    public int CurrentLikes { get; set; }
}
