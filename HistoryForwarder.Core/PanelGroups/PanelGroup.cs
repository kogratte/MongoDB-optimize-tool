using MongoDB.Bson.Serialization.Attributes;

namespace HistoryForwarder.Core.PanelGroups
{
    /// <summary>
    /// Class that represents the stored panel group document 
    /// </summary>
    public class PanelGroup
    {
        [BsonElement("PanelGroupId")]
        public int PanelGroupId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description EN.
        /// </value>
        [BsonElement("DescriptionFr")]
        public string DescriptionFr { get; set; }
        /// <value>
        /// The description EN.
        /// </value>
        [BsonElement("DescriptionEn")]
        public string DescriptionEn { get; set; }
        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>
        /// The distance.
        /// </value>
        [BsonElement("Distance")]
        public int Distance { get; set; }
        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        /// <value>
        /// The distance.
        /// </value>
        [BsonElement("DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}
