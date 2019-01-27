using HistoryForwarder.Core.PanelGroupScreens;
using HistoryForwarder.Core.TravelInfoScreens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace HistoryForwarder.Core.DocumentImporter
{
    public class TravelInfoScreensDocumentImporter : IHistoryDocumentImporter<TravelInfoScreensDocument>
    {
        private Options options;
        private readonly IMongoCollection<TravelInfoScreensDocument> previousCollection;
        private List<TravelInfoScreensDocument> documents;
        private readonly IMongoCollection<TravelInfoScreensDocument> newCollection;
        private readonly ISizeRenderer sizeRenderer;
        private readonly IAzureBlobRepository azureBlobStorage;
        private readonly IMongoDatabase mongoDb;
        private readonly IContentCompressor compressor;

        public TravelInfoScreensDocumentImporter(IAzureBlobRepository azureBlobStorage, IMongoDatabase mongoDb, IContentCompressor compressor, IUnityContainer container, ISizeRenderer sizeRenderer)
        {
            this.sizeRenderer = sizeRenderer;
            this.azureBlobStorage = azureBlobStorage;
            this.mongoDb = mongoDb;
            this.compressor = compressor;

            this.newCollection = container.Resolve<IMongoCollection<TravelInfoScreensDocument>>("NewTravelInfoScreensCollection");
            this.previousCollection = container.Resolve<IMongoCollection<TravelInfoScreensDocument>>("PreviousTravelInfoScreensCollection");

            IQueryable<TravelInfoScreensDocument> documentQuery = previousCollection.AsQueryable().Where(i => i.SlotDate >= new DateTime(2019, 01, 01));
            this.documents = documentQuery.ToList();
        }

        public async Task Process(Options options)
        {
            this.options = options;
            Console.WriteLine("Import LTI screens");

            if (this.options.CleanupDistCollection)
            {
                await this.CleanupDistCollectionAsync();
            }

            if (this.options.Verbose)
            {
                Console.WriteLine($"{documents.Count} documents to import");
            }

            if (documents.Any() && this.options.CompressContent)
            {
                await this.CompressContent();
            }
            if (documents.Any() && this.options.UseBlobStorage)
            {
                await this.MoveToAzureBlobStorageAsync();
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
                Console.WriteLine("Panel group screen import is done.");
            }
        }

        private async Task MoveToAzureBlobStorageAsync()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Transfert content to azure blob storage");
            }

            if (this.options.Verbose)
            {
                Console.WriteLine("Store to azure blob storage");
            }

            for (var i = 0; i < this.documents.Count; i++)
            {
                if (this.options.Verbose)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"{i + 1} / {this.documents.Count}");
                }

                var document = documents[i];
                var enScreen = document.EnScreen;
                var frScreen = document.FrScreen;
                var enContentUri = await this.StoreContentAsync($"LTI-{document.Id}-EN", enScreen);
                var frContentUri = await this.StoreContentAsync($"LTI-{document.Id}-FR", frScreen);

                document.EnScreen = enContentUri;
                document.FrScreen = frContentUri;
            }

            if (this.options.Verbose) Console.WriteLine(string.Empty);
            if (this.options.Verbose) Console.WriteLine("Done");
        }

        private async Task CompressContent()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Compressing content");
            }
            var totalDocumentsSize = 0;
            var totalCompressedDocumentsSize = 0;

            for (var i = 0; i < this.documents.Count; i++)
            {
                if (this.options.Verbose)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"{i + 1} / {this.documents.Count}");
                }

                var document = documents[i];
                var enScreen = document.EnScreen;
                var frScreen = document.FrScreen;

                totalDocumentsSize += document.EnScreen.Length;
                totalDocumentsSize += document.FrScreen.Length;

                enScreen = compressor.CompressContent(document.EnScreen, CompressionAlgorithm.DEFAULT);
                frScreen = compressor.CompressContent(document.FrScreen, CompressionAlgorithm.DEFAULT);

                totalCompressedDocumentsSize += enScreen.Length;
                totalCompressedDocumentsSize += frScreen.Length;
            }
            if (this.options.Verbose) Console.WriteLine(string.Empty);
            if (totalDocumentsSize > 0)
            {
                Console.WriteLine($"Total size before compression: {this.sizeRenderer.GetBytesReadable(totalDocumentsSize)}");
                Console.WriteLine($"Total size after compression: {this.sizeRenderer.GetBytesReadable(totalCompressedDocumentsSize)}");
                Console.WriteLine($"Compression rate: {100 - (100 * totalCompressedDocumentsSize / totalDocumentsSize)}%");
            }

            if (this.options.Verbose) Console.WriteLine("Done");
        }

        private async Task DropSourceCollectionAsync()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Drop source collection");
            }
            if (this.options.Process)
            {
                await this.mongoDb.DropCollectionAsync("TravelInfoScreens");
            }
            if (this.options.Verbose) Console.WriteLine("Done");
        }

        private async Task CleanupDistCollectionAsync()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Cleanup dist collection for TravelInfoScreens documents");
            }
            if (this.options.Process)
            {
                await newCollection.DeleteManyAsync(x => x.DocumentType == DocumentType.TravelInfoScreensDocument);
            }
            if (this.options.Verbose) Console.WriteLine("Done");
        }

        private async Task<string> StoreContentAsync(string name, string content)
        {
            if (!this.options.Process)
            {
                return await Task.FromResult($"http://fakeDocumentUrl/{name}");
            }
            return await this.azureBlobStorage.StoreAsync(content, name);
        }
    }
}
