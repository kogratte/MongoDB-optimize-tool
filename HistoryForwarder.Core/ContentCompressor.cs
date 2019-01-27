using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace HistoryForwarder.Core
{
    public interface IContentCompressor
    {
        string CompressContent(string input, CompressionAlgorithm algorithm);
    }

    public enum CompressionAlgorithm
    {
        // Useless. Do not compress if compression rate is none.
        NONE,
        // Default has the best takeTime / compression rate ratio
        DEFAULT,
        // A little bit more efficient than default, but slower.
        GZIP,
        //  The most efficient. But also the slower.
        BZIP2
    }

    public class ContentCompressor : IContentCompressor
    {
        public string CompressContent(string input, CompressionAlgorithm algorithm = CompressionAlgorithm.DEFAULT)
        {
            if (algorithm == CompressionAlgorithm.NONE)
            {
                return input;
            }
            if (algorithm == CompressionAlgorithm.DEFAULT)
            {
                return this.DefaultCompressionAsync(input).Result;
            }
            if (algorithm == CompressionAlgorithm.GZIP)
            {
                return this.CompressUsingGzip(input);
            }
            if (algorithm == CompressionAlgorithm.BZIP2)
            {
                return this.CompressUsingBzip2(input);
            }

            throw new NotSupportedException("Invalid algorithm");
        }

        private string CompressUsingBzip2(string input)
        {
             var bytes = Encoding.UTF8.GetBytes(input);

            using (var msi = new MemoryStream(bytes)) {
                var outStream = new MemoryStream();
                BZip2.Compress(msi, outStream, false, 9);
                var result = Convert.ToBase64String(outStream.ToArray());
                
                return result;
            }
        }

        private async Task<string> DefaultCompressionAsync(string input)
        {
             var bytes = Encoding.UTF8.GetBytes(input);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                     msi.CopyTo(gs);
                }
                var result = Convert.ToBase64String(mso.ToArray());
                
                return result;
            }
        }

        private string CompressUsingGzip(string uncompressedContent)
        {
             var bytes = Encoding.UTF8.GetBytes(uncompressedContent);

            using (var msi = new MemoryStream(bytes)) {
                var outStream = new MemoryStream();
                GZip.Compress(msi, outStream, false, 512, 9);
                var result = Convert.ToBase64String(outStream.ToArray());
                
                return result;
            }
        }

    }
}
