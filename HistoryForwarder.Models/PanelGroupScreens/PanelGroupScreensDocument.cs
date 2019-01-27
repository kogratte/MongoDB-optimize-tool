using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace HistoryForwarder.Models.PanelGroupScreens
{
    /// <summary>
    /// Class that represents the panelGroup display at a time
    /// </summary>
    public class PanelGroupScreensDocument
    {
        [BsonId(IdGenerator = typeof(PanelGroupScreensIdGenerator))]
        public string Id { get; set; }
        [BsonElement("SlotDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SlotDate { get; set; }
        [BsonElement("PanelGroupId")]
        public int PanelGroupId { get; set; }
        [BsonElement("Screens")]
        public IEnumerable<string> Screens { get; set; }

        [BsonElement("PartitionKey")]
        public string PartitionKey
        {
            get
            {
                return $"{DocumentType.PanelGroupScreensDocument}{this.SlotDate.Month}{this.SlotDate.Year}";
            }
        }
    }
}
