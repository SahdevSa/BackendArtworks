using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using ArtworkProvider.Backend.Models;

namespace ArtworkProvider.Backend.Services;

public class CampaignService
{
    private readonly IMongoCollection<CampaignModel> _CampaignCollection;

    public CampaignService(IOptions<MongoDBSettings> MongoDBSettings)
    {
        MongoClient client = new MongoClient(MongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(MongoDBSettings.Value.DatabaseName);
        _CampaignCollection = database.GetCollection<CampaignModel>(MongoDBSettings.Value.CampaignsCollection);
    }

    public async Task CreateCampaign(CampaignModel CampaignModel)
    {
        await _CampaignCollection.InsertOneAsync(CampaignModel);
        return;
    }

    public async Task<List<CampaignModel>> GetCampaigns()
    {
        return await _CampaignCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task UpdateCampaign(string id, CampaignModel CampaignModel)
    {
        string CampaignName = CampaignModel.Name;
        DateTime CampaignStart = CampaignModel.Start;
        DateTime CampaignEnd = CampaignModel.End;
        byte CampaignType = CampaignModel.Type;
        byte CampaignStatus = CampaignModel.Status;

        FilterDefinition<CampaignModel> filter = Builders<CampaignModel>.Filter.Eq("Id", id);
        UpdateDefinition<CampaignModel> update = Builders<CampaignModel>.Update.Set("Name", CampaignName)
                                                                                .Set("Start", CampaignStart)
                                                                                .Set("End", CampaignEnd)
                                                                                .Set("Type", CampaignType)
                                                                                .Set("Status", CampaignStatus);
        await _CampaignCollection.UpdateOneAsync(filter, update);
        return;
    }

    public async Task<CampaignModel> GetSingleCampaigns(string id)
    {
        FilterDefinition<CampaignModel> filter = Builders<CampaignModel>.Filter.Eq("Id", id);
        return await _CampaignCollection.Find(filter).FirstOrDefaultAsync();
    }
}