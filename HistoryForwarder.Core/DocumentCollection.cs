using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace HistoryForwarder.Core
{
    public class DocumentCollection<TDocument> : IDocumentCollection<TDocument>
    {
        private readonly IMongoCollection<TDocument> _collection;

        public DocumentCollection(IMongoCollection<TDocument> collection)
        {
            _collection = collection;
        }

        public IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public void Create(TDocument document)
        {
            _collection.InsertOne(document);
        }

        public void Drop()
        {
            _collection.Database.DropCollection(_collection.CollectionNamespace.CollectionName);
        }
    }

    public interface IDocumentCollection<TDocument>
    {
        IQueryable<TDocument> AsQueryable();

        void Create(TDocument document);
        void Drop();
    }
}
