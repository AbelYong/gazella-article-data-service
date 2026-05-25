namespace ArticleServiceTests.Data;

public static class DbConstants
{
    /// <summary>
    /// The image for the MongoDb TestContainer. Note: 8.0.21-ubi8 (slim and not slim) do not work.
    /// (The test end up hanging) Do not change it unless you have tested a different image first
    /// </summary>
    public const string DbImage = "mongodb/mongodb-community-server:8.0.21-ubi9";
    public const string DbName = "GazellaDb";
}