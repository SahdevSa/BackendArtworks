using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using ArtworkProvider.Backend.Models;

namespace ArtworkProvider.Backend.Services;

public class TaskService
{
    private readonly IMongoCollection<TaskModel> _TasksCollection;

    public TaskService(IOptions<MongoDBSettings> MongoDBSettings)
    {
        MongoClient client = new MongoClient(MongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(MongoDBSettings.Value.DatabaseName);
        _TasksCollection = database.GetCollection<TaskModel>(MongoDBSettings.Value.TasksCollection);
    }

    public async Task CreateTask(TaskModel TaskModel)
    {
        await _TasksCollection.InsertOneAsync(TaskModel);
        return;
    }

    public async Task<List<TaskModel>> GetTasks()
    {
        return await _TasksCollection.Find(new BsonDocument()).ToListAsync();
    }
}