namespace ArticleService.Data;

public static class EntitySizeConstraints
{
    public static readonly int IdLength = 36;
    
    public static readonly int ArticleTitleMaxLength = 128;
    public static readonly int ArticleCoverUriMaxLength = 256;
    public static readonly int ArticleSummaryMaxLength = 500;
    public static readonly int ArticleCategoryMaxLength = 64;
    
    public static readonly int AuthorNameMaxLength = 64;
    public static readonly int AuthorProfilePictureUriMaxLength = 256;
    
    public static readonly int CategoryNameMaxLength = 64;

    public static readonly int ReviewRejectionReasonMaxLength = 1000;
    
    public static readonly int CommentContentMaxLength = 1000;
}