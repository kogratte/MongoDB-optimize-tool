using System.Collections.Generic;
using System.Threading.Tasks;
using HistoryForwarder.Core.PanelGroups;
using ET.IsisCom.Infrastructure.Core.Business;

namespace HistoryForwarder.Core.Interfaces
{
    /// <summary>
    /// Interface to request the panel groups from cis
    /// </summary>
    public interface IPanelGroupRequestClient
    {
        /// <summary>
        /// Get the current list of cis panel groups
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        Task<IEnumerable<PanelGroup>> Get(Terminal terminal, Activity activity);
    }
}
