using System;
using ET.IsisCom.Infrastructure.Core.Business;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HistoryForwarder.Core.TravelInfoScreens
{
    public class TravelInfoScreensDocument
    {
        [BsonId(IdGenerator = typeof(TravelInfoScreensIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("SlotDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SlotDate { get; set; }
        [BsonElement("Terminal")]
        public Terminal Terminal { get; set; }
        [BsonElement("Activity")]
        public Activity Activity { get; set; }
        [BsonElement("FrScreen")]
        public string FrScreen { get; set; }
        [BsonElement("EnScreen")]
        public string EnScreen { get; set; }
        
        [BsonElement("DocumentType")]
        public readonly DocumentType DocumentType = DocumentType.TravelInfoScreensDocument;

        [BsonElement("PartitionKey")]
        public string PartitionKey
        {
            get
            {
                return $"{DocumentType}{this.SlotDate.Month}{this.SlotDate.Year}";
            }
        }
    }
}
