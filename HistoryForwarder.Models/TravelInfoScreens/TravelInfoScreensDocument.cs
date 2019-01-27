using System;
using ET.IsisCom.Infrastructure.Core.Business;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HistoryForwarder.Core.TravelInfoScreens
{
    public class TravelInfoScreensDocument
    {
        [BsonId(IdGenerator = typeof(TravelInfoScreensIdGenerator))]
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
        [BsonElement("PartitionKey")]
        public string PartitionKey
        {
            get
            {
                return $"{DocumentType.TravelInfoScreensDocument}{this.SlotDate.Month}{this.SlotDate.Year}";
            }
        }

    }
}
