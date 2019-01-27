using System.Threading.Tasks;

namespace HistoryForwarder.Core.DocumentImporter
{
    public interface IHistoryDocumentImporter<T>
    {
        Task Process(Options options);
    }
}
