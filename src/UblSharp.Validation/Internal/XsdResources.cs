using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace UblSharp.Validation.Internal
{
    /// <summary>
    /// Gives access to the XSD files embedded in a single solid gzip archive (see PackXsd target in the csproj).
    /// The archive is decompressed once, on first use.
    /// </summary>
    internal static class XsdResources
    {
        private const string ArchiveResourceName = "UblSharp.Validation.Resources.xsd.pack.gz";
        private static readonly object s_lock = new object();
        private static Dictionary<string, byte[]> s_files;

        /// <summary>Paths use '/' as separator, e.g. "maindoc/UBL-Invoice-2.1.xsd".</summary>
        public static IEnumerable<string> Paths
        {
            get { return Load().Keys; }
        }

        public static Stream Open(string path)
        {
            byte[] bytes;
            return Load().TryGetValue(path, out bytes) ? new MemoryStream(bytes, false) : null;
        }

        private static Dictionary<string, byte[]> Load()
        {
            lock (s_lock)
            {
                if (s_files != null) return s_files;

                var files = new Dictionary<string, byte[]>(StringComparer.Ordinal);
                using (var raw = typeof(XsdResources).Assembly.GetManifestResourceStream(ArchiveResourceName))
                {
                    if (raw == null)
                    {
                        throw new InvalidOperationException($"Error while getting manifest resource '{ArchiveResourceName}'");
                    }

                    using (var gzip = new GZipStream(raw, CompressionMode.Decompress))
                    using (var reader = new BinaryReader(gzip))
                    {
                        var count = reader.ReadInt32();
                        for (var i = 0; i < count; i++)
                        {
                            var path = reader.ReadString();
                            files[path] = reader.ReadBytes(reader.ReadInt32());
                        }
                    }
                }

                s_files = files;
                return files;
            }
        }
    }
}
