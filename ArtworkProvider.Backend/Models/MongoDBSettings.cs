namespace ArtworkProvider.Backend.Models;

public class MongoDBSettings
{

    public string ConnectionURI { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string UsersCollection { get; set; } = null!;
    public string CompanyCollection { get; set; } = null!;
    public string CampaignsCollection { get; set; } = null!;
    public string TasksCollection { get; set; } = null!;
    public string SprintsCollection { get; set; } = null!;

}