using System;
using System.Collections.Generic;
using ET.IsisCom.Infrastructure.Core.Business;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HistoryForwarder.Core.PanelGroups
{
    public class PanelGroupsDocument
    {
        public PanelGroupsDocument()
        {
            Items = new List<PanelGroup>();
        }

        [BsonId(IdGenerator = typeof(PanelGroupIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("SlotDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SlotDate { get; set; }
        /// <summary>
        /// Gets or sets the terminal.
        /// </summary>
        /// <value>
        /// The terminal.
        /// </value>
        [BsonElement("Terminal")]
        public Terminal Terminal { get; set; }

        /// <summary>
        /// Gets or sets the activity
        /// </summary>
        [BsonElement("Activity")]
        public Activity Activity { get; set; }
        [BsonElement("Items")]
        public IList<PanelGroup> Items { get; set; }

        [BsonElement("DocumentType")]
        public readonly DocumentType DocumentType = DocumentType.PanelGroupsDocument;

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
