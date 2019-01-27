using HistoryForwarder.Core.PanelGroups;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace HistoryForwarder.Core.DocumentImporter
{
    public class PanelGroupsDocumentImporter: IHistoryDocumentImporter<PanelGroupsDocument>
    {
        private Options options;
        private readonly IMongoCollection<PanelGroupsDocument> previousCollection;
        private List<PanelGroupsDocument> documents;
        private readonly IMongoCollection<PanelGroupsDocument> newCollection;
        private readonly IAzureBlobRepository azureBlobStorage;
        private readonly IMongoDatabase mongoDb;
        private readonly IContentCompressor compressor;

        public PanelGroupsDocumentImporter(IAzureBlobRepository azureBlobStorage, IMongoDatabase mongoDb, IContentCompressor compressor, IUnityContainer container)
        {
            this.azureBlobStorage = azureBlobStorage;
            this.mongoDb = mongoDb;
            this.compressor = compressor;

            this.newCollection = container.Resolve<IMongoCollection<PanelGroupsDocument>>("NewPanelGroupCollection");
            this.previousCollection = container.Resolve<IMongoCollection<PanelGroupsDocument>>("PreviousPanelGroupCollection");

            this.documents = previousCollection.AsQueryable().ToList();
        }


        public async Task Process(Options options)
        {
            this.options = options;

            if (this.options.CleanupDistCollection)
            {
                await this.CleanupDistCollectionAsync();
            }

            if (this.options.Verbose)
            {
                Console.WriteLine($"{documents.Count} documents to import");
            }

            if (documents.Any())
            {
                if (this.options.Process)
                {
                    await newCollection.InsertManyAsync(documents);
                }

            }
            if (this.options.DropSourceCollection)
            {
                await this.DropSourceCollectionAsync();
            }
            if (this.options.Verbose)
            {
                Console.WriteLine("Panel groups import is done.");
            }
        }

        private async Task DropSourceCollectionAsync()
        {
            if (options.Verbose)
            {
                Console.WriteLine("Drop PanelGroup collection");
            }
            if (options.Process)
            {
                await this.mongoDb.DropCollectionAsync("PanelGroup");
            }
            if (this.options.Verbose) Console.WriteLine("Done");
        }

        private async Task CleanupDistCollectionAsync()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Cleaning target collection for PanelGroup documents");
            }
            if (this.options.Process)
            {
                await newCollection.DeleteManyAsync(x => x.DocumentType == DocumentType.PanelGroupsDocument);
            }
            if (this.options.Verbose) Console.WriteLine("Done");
        }
    }
}
