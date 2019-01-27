using HistoryForwarder.Core;
using HistoryForwarder.Core.DocumentImporter;
using HistoryForwarder.Core.PanelGroups;
using HistoryForwarder.Core.PanelGroupScreens;
using HistoryForwarder.Core.TravelInfoScreens;
using System;
using System.Threading.Tasks;

namespace HistoryForwarder
{
    public interface IImporterService
    {
        Task ImportAsync(Options options);
    }

    public class ImporterService : IImporterService
    {
        private readonly IHistoryDocumentImporter<TravelInfoScreensDocument> ltiImporter;
        private readonly IHistoryDocumentImporter<PanelGroupsDocument> panelGroupsImporter;
        private readonly IHistoryDocumentImporter<PanelGroupScreensDocument> panelGroupScreensImporter;

        public ImporterService(IHistoryDocumentImporter<TravelInfoScreensDocument> ltiImporter, 
            IHistoryDocumentImporter<PanelGroupScreensDocument> panelGroupScreensImporter,
            IHistoryDocumentImporter<PanelGroupsDocument> panelGroupsImporter)
        {
            this.ltiImporter = ltiImporter;
            this.panelGroupsImporter = panelGroupsImporter;
            this.panelGroupScreensImporter = panelGroupScreensImporter;
        }

        public async Task ImportAsync(Options options)
        {
            if (!options.Process)
            {
                Console.WriteLine("... SIMULATION IN PROGRESS ...");
            }

            await this.panelGroupsImporter.Process(options);
            await this.panelGroupScreensImporter.Process(options);
            await this.ltiImporter.Process(options);

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }
}
