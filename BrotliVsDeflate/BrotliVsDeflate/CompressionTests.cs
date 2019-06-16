using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BrotliVsDeflate
{
    public static class CompressionTests
    {
        public static int Deflate(CompressionLevel level, ref TimeSpan span)
        {
            byte[] data = CompressionExamples.MarbibmUncompressed;
            MemoryStream ms = new MemoryStream((int)(data.Length * 1.2));

            Stopwatch sw = Stopwatch.StartNew();

            using (DeflateStream stream = new DeflateStream(ms, level, true))
                stream.Write(CompressionExamples.MarbibmUncompressed, 0, data.Length);

            span += sw.Elapsed;

            return (int)ms.Position;
        }

        public static int Brotli(int quality, int window, ref TimeSpan span)
        {
            int bytesConsumed;
            int bytesWritten;

            BrotliEncoder encoder = new BrotliEncoder(quality, window);

            byte[] data = CompressionExamples.MarbibmUncompressed;
            byte[] destination = new byte[(int)(data.Length * 1.2)];

            Stopwatch sw = Stopwatch.StartNew();

            encoder.Compress(data.AsSpan(), destination.AsSpan(), out bytesConsumed, out bytesWritten, true);

            span += sw.Elapsed;

            return bytesWritten;
        }
    }
}
