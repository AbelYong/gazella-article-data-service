using ArticleService.Data;
using ArticleService.Data.Repositories;
using ArticleService.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ArticleService.Services;
using ArticleService.Services.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
var mongoDbName = builder.Configuration.GetValue<string>("MongoDbName");

builder.Services.AddDbContext<GazellaDbContext>(options =>
    options.UseMongoDB(mongoConnectionString ?? throw new InvalidOperationException("Connection string is missing"),
        mongoDbName ?? "DB_NAME_NOT_PROVIDED"));

builder.Services.AddScoped<IDraftRepository, DraftRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IInteractionRepository, InteractionRepository>();

var app = builder.Build();

app.MapGrpcService<DraftService>();
app.MapGrpcService<ArticleService.Services.ArticleService>();
app.MapGrpcService<ReviewService>();
app.MapGrpcService<InteractionService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
