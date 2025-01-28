using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedKernel.Models
{
    public class Medicine
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 