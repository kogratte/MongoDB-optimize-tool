using System;

namespace HistoryForwarder.Core
{
    /// <summary>
    /// Class that represents the reference to a document 
    /// </summary>
    public class DocumentReference
    {
        /// <summary>
        /// Link to the document
        /// </summary>
        public string DocumentId { get; set; }
        /// <summary>
        /// Recording date
        /// </summary>
        public DateTime SlotDate { get; set; }
    }
}
