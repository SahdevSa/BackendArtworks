using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using ArtworkProvider.Backend.Models;

namespace ArtworkProvider.Backend.Services;

public class CompanyService
{
    private readonly IMongoCollection<CompanyModel> _CompanyCollection;

    public CompanyService(IOptions<MongoDBSettings> MongoDBSettings)
    {
        MongoClient client = new MongoClient(MongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(MongoDBSettings.Value.DatabaseName);
        _CompanyCollection = database.GetCollection<CompanyModel>(MongoDBSettings.Value.CompanyCollection);
    }

    public async Task CreateCompany(CompanyModel CompanyModel)
    {
        await _CompanyCollection.InsertOneAsync(CompanyModel);
        return;
    }

    public async Task<List<CompanyModel>> GetCompany()
    {
        return await _CompanyCollection.Find(new BsonDocument()).ToListAsync();
    }
}