using System;
using System.Collections.Generic;
using System.Linq;
using HistoryForwarder.Core.Interfaces;

namespace HistoryForwarder.Core.PanelGroupScreens
{
    /// <summary>
    /// Implementation of <see cref="IPanelGroupScreensRepository"/>
    /// </summary>
    public class PanelGroupScreensRepository : IPanelGroupScreensRepository
    {
        private readonly IDocumentCollection<PanelGroupScreensDocument> _documentCollection;

        public PanelGroupScreensRepository(IDocumentCollection<PanelGroupScreensDocument> documentCollection)
        {
            _documentCollection = documentCollection;
        }

        public void Create(PanelGroupScreensDocument document)
        {
            _documentCollection.Create(document);
        }

        public DocumentReference Select(int panelGroupId, DateTime atTime)
        {
            var queryablePanelGroup = _documentCollection.AsQueryable();
            var query = queryablePanelGroup.Where(s => s.PanelGroupId == panelGroupId && s.SlotDate <= atTime)
                .OrderByDescending(p => p.SlotDate)
                .Take(1);
            
            var result = query.ToList().Select(p => new DocumentReference()
            {
                DocumentId = p.Id,
                SlotDate = p.SlotDate
            }).FirstOrDefault();

            return result;
        }

        public PanelGroupScreensDocument Select(string documentId)
        {
            var queryablePanelGroup = _documentCollection.AsQueryable();
            return queryablePanelGroup.FirstOrDefault(d => d.Id == documentId);
        }

        public void Drop()
        {
            _documentCollection.Drop();
        }
    }
}
