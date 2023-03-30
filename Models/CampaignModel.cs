using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ArtworkProvider.Backend.Models;

public class CampaignModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Start { get; set; } = new DateTime()!;

    public DateTime End { get; set; } = new DateTime()!;

    public byte Status { get; set; } = 0!;

    public byte Type {get; set; } =0!;


}