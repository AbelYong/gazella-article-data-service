using ArticleService.Data;
using ArticleService.Data.Repositories.Implementations;
using ArticleService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.MongoDb;

namespace ArticleServiceTests.Data.Repositories;

[TestFixture]
public class CategoryRepositoryTest
{
    private MongoDbContainer _container;
    private GazellaDbContext _context;
    private CategoryRepository _repository;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _container = new MongoDbBuilder(DbConstants.DbImage)
            .WithReplicaSet()
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<GazellaDbContext>()
            .UseMongoDB(_container.GetConnectionString(), DbConstants.DbName)
            .Options;
        
        _context = new GazellaDbContext(options);

        _repository = new CategoryRepository(_context, new NullLogger<CategoryRepository>());
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }

    [Test]
    public async Task AddCategory()
    {
        var testCategory = new Category { Name = "addTest" };
        var id = testCategory.Id;
        
        var newCategory = await _repository.AddCategoryAsync(testCategory);
        
        Assert.That(newCategory.Id, Is.EqualTo(id));
    }
    
    [Test]
    public async Task GetAllCategories()
    {
        await _repository.DeleteAllCategories();
        var categoryOne = new Category { Name = "test" };
        var categoryTwo = new Category { Name = "test2" };
        await _repository.AddCategoryAsync(categoryOne);
        await _repository.AddCategoryAsync(categoryTwo);
        
        var categories = await _repository.GetCategoriesAsync();
        
        Assert.That(categories.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetCategoryById()
    {
        var testCategory = new Category { Name = "test" };
        var id = testCategory.Id;
        await _repository.AddCategoryAsync(testCategory);
        
        var dbCategory = await _repository.GetCategoryByIdAsync(id);
        
        Assert.That(dbCategory.Id, Is.EqualTo(id));
    }
    
    [Test]
    public async Task DeleteAllCategories()
    {
        var testCategory = new Category { Name = "test" };
        await _repository.AddCategoryAsync(testCategory);
        
        await _repository.DeleteAllCategories();
        var categories = await _repository.GetCategoriesAsync();
        
        Assert.That(categories.Count, Is.EqualTo(0));
    }
}
