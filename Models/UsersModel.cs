using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ArtworkProvider.Backend.Models;

[BsonIgnoreExtraElements]
public class UsersModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;
    public DateTime SignupDate { get; set; } = new DateTime()!;
    public DateTime RemoveDate { get; set; } = new DateTime()!;


    public bool Active { get; set; } = true!;

    [BsonRepresentation(BsonType.ObjectId)]
    public List<string> CampaignId { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string CompanyId { get; set; } = null!;

}
