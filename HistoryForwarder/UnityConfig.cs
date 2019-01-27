using HistoryForwarder.Core;
using HistoryForwarder.Core.DocumentImporter;
using HistoryForwarder.Core.Interfaces;
using HistoryForwarder.Core.PanelGroups;
using HistoryForwarder.Core.PanelGroupScreens;
using HistoryForwarder.Core.TravelInfoScreens;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Security.Authentication;
using Unity;
using Unity.Injection;
using Unity.RegistrationByConvention;

namespace HistoryForwarder
{
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        public static IUnityContainer GetConfiguredContainer()
        {
            return UnityConfig.container.Value;
        }

        private static void RegisterTypes(UnityContainer container)
        {
            container.RegisterSingleton<MongoClient>(new InjectionFactory(c =>
            {
                MongoClientSettings settings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(Config.IsiscomManagerDataBaseEndPoint, 10255),
                    UseSsl = true,
                    SslSettings = new SslSettings()
                };
                settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
                MongoIdentity identity = new MongoInternalIdentity(Config.IsiscomManagerDatabaseName, Config.IsiscomManagerDatabaseUser);
                MongoIdentityEvidence evidence = new PasswordEvidence(Config.IsiscomManagerDatabasePrimaryKey);
                settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
                return new MongoClient(settings);
            }));

            container.RegisterSingleton<IMongoDatabase>(new InjectionFactory(c =>
            {
                var client = c.Resolve<MongoClient>();
                return client.GetDatabase(Config.IsiscomManagerDatabaseName);
            }));

            container.RegisterType<IImporterService, ImporterService>();

            RegisterPreviousCollections(container);
            RegisterNewCollections(container);
            container.RegisterType<IContentCompressor, ContentCompressor>();
            container.RegisterInstance<IAzureBlobRepository>(new AzureBlobRepository());
            container.RegisterType<IHistoryDocumentImporter<TravelInfoScreensDocument>, TravelInfoScreensDocumentImporter>();
            container.RegisterType<IHistoryDocumentImporter<PanelGroupScreensDocument>, PanelGroupScreensDocumentsImporter>();
            container.RegisterType<IHistoryDocumentImporter<PanelGroupsDocument>, PanelGroupsDocumentImporter>();
            container.RegisterType<ISizeRenderer, SizeRenderer>();
        }

        private static void RegisterPreviousCollections(UnityContainer container)
        {
            //panel groups screens
            container.RegisterSingleton<IMongoCollection<PanelGroupScreensDocument>>("PreviousPanelGroupScreensCollection", new InjectionFactory(c =>
            {
                var db = c.Resolve<IMongoDatabase>();

                return db.GetCollection<PanelGroupScreensDocument>("PanelGroupScreens");
            }));

            //panel group
            container.RegisterSingleton<IMongoCollection<PanelGroupsDocument>>("PreviousPanelGroupCollection", new InjectionFactory(c =>
            {
                var db = c.Resolve<IMongoDatabase>();
                return db.GetCollection<PanelGroupsDocument>("PanelGroup");
            }));

            //travelinfo
            container.RegisterSingleton<IMongoCollection<TravelInfoScreensDocument>>("PreviousTravelInfoScreensCollection", new InjectionFactory(c =>
            {
                var db = c.Resolve<IMongoDatabase>();
                return db.GetCollection<TravelInfoScreensDocument>("TravelInfoScreens");
            }));
        }

        private static void RegisterNewCollections(IUnityContainer container)
        {
            //panel groups screens
            container.RegisterSingleton<IMongoCollection<PanelGroupScreensDocument>>("NewPanelGroupScreensCollection", new InjectionFactory(c =>
            {
                var db = c.Resolve<IMongoDatabase>();

                return db.GetCollection<PanelGroupScreensDocument>("HistoryDocuments");
            }));

            //panel group
            container.RegisterSingleton<IMongoCollection<PanelGroupsDocument>>("NewPanelGroupCollection", new InjectionFactory(c =>
            {
                var db = c.Resolve<IMongoDatabase>();
                return db.GetCollection<PanelGroupsDocument>("HistoryDocuments");
            }));

            //travelinfo
            container.RegisterSingleton<IMongoCollection<TravelInfoScreensDocument>>("NewTravelInfoScreensCollection", new InjectionFactory(c =>
            {
                var db = c.Resolve<IMongoDatabase>();
                return db.GetCollection<TravelInfoScreensDocument>("HistoryDocuments");
            }));

        }
    }
}
