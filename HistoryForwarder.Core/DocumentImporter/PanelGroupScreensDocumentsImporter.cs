using HistoryForwarder.Core.PanelGroupScreens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Operations;

namespace HistoryForwarder.Core.DocumentImporter
{
    public class PanelGroupScreensDocumentsImporter : IHistoryDocumentImporter<PanelGroupScreensDocument>
    {
        private Options options;
        private readonly IMongoCollection<PanelGroupScreensDocument> previousCollection;
        private int totalDocumentCount;
        private long totalDocumentsSize;
        private long totalCompressedDocumentsSize;
        private ProgressBar compressProgressBar;
        private ProgressBar moveToBlobStorageProgressBar;
        private ProgressBar importProgressBar;
        private readonly IMongoCollection<PanelGroupScreensDocument> newCollection;
        private readonly IAzureBlobRepository azureBlobStorage;
        private readonly IMongoDatabase mongoDb;
        private readonly IContentCompressor compressor;
        private readonly ISizeRenderer sizeRenderer;

        public PanelGroupScreensDocumentsImporter(IAzureBlobRepository azureBlobStorage, IMongoDatabase mongoDb, IContentCompressor compressor, IUnityContainer container, ISizeRenderer sizeRenderer)
        {
            this.azureBlobStorage = azureBlobStorage;
            this.mongoDb = mongoDb;
            this.compressor = compressor;
            this.sizeRenderer = sizeRenderer;

            this.newCollection = container.Resolve<IMongoCollection<PanelGroupScreensDocument>>("NewPanelGroupScreensCollection");
            this.previousCollection = container.Resolve<IMongoCollection<PanelGroupScreensDocument>>("PreviousPanelGroupScreensCollection");

            this.totalDocumentsSize = 0;
            this.totalCompressedDocumentsSize = 0;
        }

        public async Task Process(Options options)
        {
            this.options = options;
            Console.WriteLine("Import Panel group screens");

            if (this.options.CleanupDistCollection)
            {
                await this.CleanupDistCollectionAsync();
            }

            this.totalDocumentCount = (int)await this.previousCollection.EstimatedDocumentCountAsync();
            this.importProgressBar = new ProgressBar("Import", this.totalDocumentCount);
            if (options.CompressContent)
                this.compressProgressBar = new ProgressBar("Compress", this.totalDocumentCount);
            if (options.UseBlobStorage)
                this.moveToBlobStorageProgressBar = new ProgressBar("Move to Azure Blob Storage", this.totalDocumentCount);

            
            FindOptions<PanelGroupScreensDocument> findOptions = new FindOptions<PanelGroupScreensDocument>
            {
                BatchSize = 200,
                NoCursorTimeout = true,
            };

            var progressPosition = Console.CursorTop;
            using (IAsyncCursor<PanelGroupScreensDocument> cursor = await this.previousCollection.FindAsync(Builders<PanelGroupScreensDocument>.Filter.Empty, findOptions))
            {
                while (cursor.MoveNext())
                {
                    var documents = cursor.Current.ToList();

                    if (documents.Any())
                    {
                        if (this.options.CompressContent)
                        {
                            this.CompressContent(documents);
                        }
                        if (this.options.UseBlobStorage)
                        {
                            await this.MoveToAzureBlobStorage(documents);
                        }
                        if (this.options.Process)
                        {
                            await newCollection.InsertManyAsync(documents);
                        }
                        this.importProgressBar.Update(documents.Count);
                    }
                }
            }

            if (this.options.CompressContent && totalDocumentsSize > 0)
            {
                Console.WriteLine($"Total size before compression: {this.sizeRenderer.GetBytesReadable(totalDocumentsSize)}");
                Console.WriteLine($"Total size after compression: {this.sizeRenderer.GetBytesReadable(totalCompressedDocumentsSize)}");
                Console.WriteLine($"Compression rate: {100 - (100 * totalCompressedDocumentsSize / totalDocumentsSize)}%");
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

        private async Task MoveToAzureBlobStorage(List<PanelGroupScreensDocument> documents)
        {
            for (var i = 0; i < documents.Count; i++)
            {
                var document = documents[i];
                var screens = document.Screens.ToList();
                var screenUrls = new List<string>();
                for (var j = 0; j < screens.Count; j++)
                {
                    var screen = screens[j];
                    var screenContentUri = await StoreContentAsync($"PanelGroupsScreens-{document.Id}-{j}", screen);
                    screenUrls.Add(screenContentUri);
                }
                document.Screens = screenUrls;
                this.moveToBlobStorageProgressBar.Update(1);
            };
        }

        private void CompressContent(List<PanelGroupScreensDocument> documents)
        {
            for (var i = 0; i < documents.Count; i++)
            {
                var document = documents[i];
                var screens = document.Screens.ToList();
                for (var j = 0; j < screens.Count(); j++)
                {
                    var screen = string.Copy(screens[j]);
                    totalDocumentsSize += screen.Length;

                    screens[j] = compressor.CompressContent(screen, CompressionAlgorithm.DEFAULT);
                    totalCompressedDocumentsSize += screens[j].Length;
                }
                document.Screens = screens;
                this.compressProgressBar.Update(1);
            };
        }

        private async Task DropSourceCollectionAsync()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Drop source collection");
            }
            if (this.options.Process)
            {
                await this.mongoDb.DropCollectionAsync("PanelGroupScreens");
            }
            if (this.options.Verbose) Console.WriteLine("Done");
        }

        private async Task CleanupDistCollectionAsync()
        {
            if (this.options.Verbose)
            {
                Console.WriteLine("Cleanup dist collection for PanelGroupScreens documents");
            }
            if (this.options.Process)
            {
                await newCollection.DeleteManyAsync(x => x.DocumentType == DocumentType.PanelGroupScreensDocument);
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
