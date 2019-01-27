using System.Configuration;

namespace HistoryForwarder.Core
{
    public static class Config
    {
        public static string IsiscomManagerDataBaseEndPoint => ConfigurationManager.AppSettings["Manager.Database.Endpoint"];

        public static string IsiscomManagerDatabaseName => ConfigurationManager.AppSettings["Manager.Database.Name"];

        public static string IsiscomManagerDatabasePrimaryKey => ConfigurationManager.AppSettings["Manager.Database.PrimaryKey"];

        public static string IsiscomManagerDatabaseUser => ConfigurationManager.AppSettings["Manager.Database.User"];

        public static string BlobStorageContainer => ConfigurationManager.AppSettings["Manager.BlobStorage.ContainerName"];
        public static string BlobStorageConnexionString => ConfigurationManager.AppSettings["Manager.BlobStorage.ConnexionString"];

    }
}
