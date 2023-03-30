using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using ArtworkProvider.Backend.Models;

namespace ArtworkProvider.Backend.Services;

public class SprintService
{
    private readonly IMongoCollection<SprintModel> _SprintCollection;

    public SprintService(IOptions<MongoDBSettings> MongoDBSettings)
    {
        MongoClient client = new MongoClient(MongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(MongoDBSettings.Value.DatabaseName);
        _SprintCollection = database.GetCollection<SprintModel>(MongoDBSettings.Value.SprintsCollection);
    }

    public async Task CreateSprint(SprintModel SprintModel)
    {
        await _SprintCollection.InsertOneAsync(SprintModel);
        return;
    }

    public async Task<List<SprintModel>> GetSprints()
    {
        return await _SprintCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task UpdateSprint(string id, SprintModel SprintModel)
    {
        string SprintName = SprintModel.Name;
        DateTime SprintStart = SprintModel.Start;
        DateTime SprintEnd = SprintModel.End;
        byte SprintType = SprintModel.Type;
        byte SprintStatus = SprintModel.Status;
        string SprintCampId = SprintModel.CampId;


        FilterDefinition<SprintModel> filter = Builders<SprintModel>.Filter.Eq("Id", id);
        UpdateDefinition<SprintModel> update = Builders<SprintModel>.Update.Set("Name", SprintName)
                                                                            .Set("Start", SprintStart)
                                                                            .Set("End", SprintEnd)
                                                                            .Set("Type", SprintType)
                                                                            .Set("Status", SprintStatus)
                                                                            .Set("CampId", SprintCampId);

        await _SprintCollection.UpdateOneAsync(filter, update);
        return;
    }

    public async Task<SprintModel> GetSingleSprint(string id)
    {
        FilterDefinition<SprintModel> filter = Builders<SprintModel>.Filter.Eq("Id", id);
        return await _SprintCollection.Find(filter).FirstOrDefaultAsync();
    }

}