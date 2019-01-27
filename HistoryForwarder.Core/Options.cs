using CommandLine;

namespace HistoryForwarder.Core
{
    public class Options
    {
        [Option('p', "process", HelpText = "Default is a simulation. Please use this option to process to move.")]
        public bool Process { get; set; }

        [Option('b', "useBlobStorage", Required =false, HelpText = "Transfer content to blob storage.")]

        public bool UseBlobStorage { get; set; }

        [Option('c', "compress", Required = false, HelpText = "Compress content.")]
        public bool CompressContent { get; set; }

        [Option('d', "drop", Required = false, HelpText = "Drop source collections after import.")]
        public bool DropSourceCollection { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option("cleanupDist", Required = false, HelpText = "Cleanup the target collection")]
        public bool CleanupDistCollection { get; set; }
    }
}
