using CommandLine;
using HistoryForwarder.Core;
using System;
using System.Threading.Tasks;
using Unity;

namespace HistoryForwarder
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = UnityConfig.GetConfiguredContainer();
            Parser.Default.ParseArguments<Options>(args)
                     .WithParsed<Options>(o =>
                     {
                         Console.WriteLine("Run options : ");
                         Console.WriteLine($"Verbose : {o.Verbose}");
                         Console.WriteLine($"Cleaup dist collection: {o.CleanupDistCollection}");
                         Console.WriteLine($"Drop source : {o.DropSourceCollection}");
                         Console.WriteLine($"Process : {o.Process}");
                         Console.WriteLine($"Use Azure Blob Storage : {o.UseBlobStorage}");
                         Console.WriteLine($"Compress content : {o.CompressContent}");

                         var importTask = container.Resolve<IImporterService>().ImportAsync(o);

                         Task.WaitAll(new[] { importTask });
                     });

        }
    }
}
