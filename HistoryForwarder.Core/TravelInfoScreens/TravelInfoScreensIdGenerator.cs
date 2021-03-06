﻿using System;
using HistoryForwarder.Core.TravelInfoScreens;
using MongoDB.Bson.Serialization;

namespace HistoryForwarder.Core.TravelInfoScreens
{
    /// <summary>
    /// Class that generates a readable document id
    /// </summary>
    public class TravelInfoScreensIdGenerator : IIdGenerator
    {
        /// <summary>Generates an Id for a document.</summary>
        /// <param name="container">The container of the document (will be a MongoCollection when called from the C# driver). </param>
        /// <param name="document">The document.</param>
        /// <returns>An Id.</returns>
        public object GenerateId(object container, object document)
        {
            var pgDoc = (TravelInfoScreensDocument)document;
            var timeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return $"{pgDoc.Terminal}_{pgDoc.Activity}_{timeStamp}";
        }

        /// <summary>Tests whether an Id is empty.</summary>
        /// <param name="id">The Id.</param>
        /// <returns>True if the Id is empty.</returns>
        public bool IsEmpty(object id)
        {
            return id == null || string.IsNullOrEmpty(id.ToString());
        }
    }
}