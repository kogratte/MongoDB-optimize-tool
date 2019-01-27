using System;
using System.Linq;
using HistoryForwarder.Core.Interfaces;
using ET.IsisCom.Infrastructure.Core.Business;
using System.Collections.Generic;

namespace HistoryForwarder.Core.TravelInfoScreens
{
    public class TravelInfoScreenRepository : ITravelInfoScreenRepository
    {
        private readonly IDocumentCollection<TravelInfoScreensDocument> _document;

        public TravelInfoScreenRepository(IDocumentCollection<TravelInfoScreensDocument> document)
        {
            _document = document;
        }

        public IEnumerable<TravelInfoScreensDocument> All()
        {
            return _document.AsQueryable().Where(x => true);
        }

        public void Create(TravelInfoScreensDocument document)
        {
            _document.Create(document);
        }

        public TravelInfoScreensDocument Select(string documentId)
        {
            var queryablePanelGroup = _document.AsQueryable();
            return queryablePanelGroup.FirstOrDefault(d => d.Id == documentId);
        }

        public DocumentReference Select(Terminal terminal, Activity activity, DateTime atTime)
        {
            var collection = _document.AsQueryable();
            var query = collection.Where(s => s.Terminal == terminal && s.Activity == activity && s.SlotDate <= atTime)
                .OrderByDescending(p => p.SlotDate)
                .Take(1);

            var result = query.ToList().Select(p => new DocumentReference()
            {
                DocumentId = p.Id,
                SlotDate = p.SlotDate
            }).FirstOrDefault();

            return result;
        }

        public TravelInfoScreensDocument SelectLastDocument(Terminal terminal, Activity activity)
        {
            return _document.AsQueryable()
              .Where(e => e.Activity == activity &&
                          e.Terminal == terminal)
              .OrderByDescending(o => o.SlotDate)
              .FirstOrDefault();
        }
    }
}
