using System;
using System.Collections.Generic;
using System.Linq;
using HistoryForwarder.Core.Interfaces;
using ET.IsisCom.Infrastructure.Core.Business;

namespace HistoryForwarder.Models.PanelGroups
{
    public class PanelGroupRepository : IPanelGroupRepository
    {
        private IDocumentCollection<PanelGroupsDocument> _collection;

        public PanelGroupRepository(IDocumentCollection<PanelGroupsDocument> collection)
        {
            _collection = collection;
        }

        /// <summary>
        /// Create a new <see cref="PanelGroupsDocument"/>
        /// </summary>
        /// <param name="document"></param>
        public void Create(PanelGroupsDocument document)
        {
            _collection.Create(document);
        }

        /// <summary>
        /// Select the current <see cref="PanelGroupsDocument"/>
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public PanelGroupsDocument Select(Terminal terminal, Activity activity)
        {
            return _collection.AsQueryable()
                .Where(e => e.Activity == activity &&
                            e.Terminal == terminal)
                .OrderByDescending(o => o.SlotDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// Select the screens displayed for a panelGroup between 2 dates
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="startingDate">from starting date.</param>
        /// <param name="endingDate">until the ending date.</param>
        /// <param name="terminal"></param>
        /// <remarks>https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ef/language-reference/supported-and-unsupported-linq-methods-linq-to-entities</remarks>
        /// <returns></returns>
        public IEnumerable<PanelGroupsDocument> Select(Terminal terminal, Activity activity, DateTime startingDate, DateTime endingDate)
        {
            var queryablePanelGroup = _collection.AsQueryable();

            var previous = queryablePanelGroup.Where(s => s.Terminal == terminal && s.Activity == activity && s.SlotDate < startingDate)
                .OrderByDescending(p => p.SlotDate)
                .Take(1);

            var interval = queryablePanelGroup.Where(s =>
                    s.Terminal == terminal && s.Activity == activity && s.SlotDate >= startingDate &&
                    s.SlotDate <= endingDate)
                .ToList(); //union of 2 IQueryable not supported

            return interval
                .Union(previous)
                .OrderBy(d => d.SlotDate)
                .ToList();
        }
    }
}
