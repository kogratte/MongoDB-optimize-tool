using System;
using System.Collections.Generic;
using HistoryForwarder.Core.PanelGroups;
using ET.IsisCom.Infrastructure.Core.Business;

namespace HistoryForwarder.Core.Interfaces
{
    /// <summary>
    /// The interface that manages the panel group history
    /// </summary>
    public interface IPanelGroupRepository
    {
        /// <summary>
        /// Create a new <see cref="PanelGroupsDocument"/>
        /// </summary>
        /// <param name="document"></param>
        void Create(PanelGroupsDocument document);

        /// <summary>
        /// Select the current <see cref="PanelGroupsDocument"/>
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        PanelGroupsDocument Select(Terminal terminal, Activity activity);

        /// <summary>
        /// Sekect the list of <see cref="PanelGroupsDocument"/> between 2 dates 
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="activity"></param>
        /// <param name="startingDate"></param>
        /// <param name="endingDate"></param>
        /// <returns></returns>
        IEnumerable<PanelGroupsDocument> Select(Terminal terminal, Activity activity, DateTime startingDate,
            DateTime endingDate);
    }
}
