using System;
using HistoryForwarder.Core.TravelInfoScreens;
using ET.IsisCom.Infrastructure.Core.Business;
using System.Collections.Generic;

namespace HistoryForwarder.Core.Interfaces
{
    /// <summary>
    /// The interface that manages the travelinfo screens history
    /// </summary>
    public interface ITravelInfoScreenRepository
    {
        void Create(TravelInfoScreensDocument document);
        TravelInfoScreensDocument Select(string documentId);
        DocumentReference Select(Terminal terminal, Activity activity, DateTime atTime);
        TravelInfoScreensDocument SelectLastDocument(Terminal terminal, Activity activity);

        IEnumerable<TravelInfoScreensDocument> All();
    }
}
