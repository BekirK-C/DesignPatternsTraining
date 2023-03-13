using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Strategy.Models;

namespace WebApp.Strategy.Models
{
    public class Product
    {
        [BsonId] // MongoDB için
        [Key] // EF Core için
        [BsonRepresentation(BsonType.ObjectId)]
        
        public string? Id { get; set; }
        public string Name { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? UserId { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }
    }
}

// Product sınıfında, [BsonRepresentation(BsonType.ObjectId)] özniteliği, Id özelliğinin ObjectId tipi için nasıl temsil edileceğini belirtir.
// Bu, MongoDB veritabanındaki _id alanının ObjectId tipi ile temsil edilmesini sağlar.