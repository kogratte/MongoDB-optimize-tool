using System;
using HistoryForwarder.Core.PanelGroupScreens;

namespace HistoryForwarder.Core.Interfaces
{
    /// <summary>
    /// The interface that manages the panel group screens history
    /// </summary>
    public interface IPanelGroupScreensRepository
    {
        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="document"></param>
        void Create(PanelGroupScreensDocument document);
        /// <summary>
        /// Drop the history
        /// </summary>
        void Drop();
        DocumentReference Select(int panelGroupId, DateTime atTime);
        PanelGroupScreensDocument Select(string documentId);
    }
}
