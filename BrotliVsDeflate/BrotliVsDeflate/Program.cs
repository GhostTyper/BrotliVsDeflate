using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;

namespace BrotliVsDeflate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(1, 1);
            Console.SetBufferSize(130, 19);
            Console.SetWindowSize(130, 19);

            Console.CursorVisible = false;

            Dictionary<CompressionLevel, TestResult> results = new Dictionary<CompressionLevel, TestResult>();

            TimeSpan span = new TimeSpan(0);
            int size = 0;
            int uncompressedSize = CompressionExamples.MarbibmUncompressed.Length;

            foreach (CompressionLevel level in new CompressionLevel[] { CompressionLevel.NoCompression, CompressionLevel.Fastest, CompressionLevel.Optimal })
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(10);

                    size = CompressionTests.Deflate(level, ref span);
                }

                span = new TimeSpan(0);

                for (int i = 0; i < 7; i++)
                {
                    Thread.Sleep(10);

                    size = CompressionTests.Deflate(level, ref span);
                }

                results.Add(level, new TestResult(size, span / 7));
            }

            Console.WriteLine();
            Console.WriteLine($"  Brotli compressed size compared to Deflate Fastest in Bytes: {uncompressedSize} -> {results[CompressionLevel.Fastest].Size}.\n");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;

            TestResult[,] resultTable = new TestResult[15, 12];

            PrintCenter("Win\\Qual");

            for (int quality = 0; quality <= 11; quality++)
                PrintCenter(quality.ToString());

            for (int window = 10; window <= 24; window++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                PrintRight((Math.Pow(2, window) - 16).ToString());

                Console.ForegroundColor = ConsoleColor.White;

                for (int quality = 0; quality <= 11; quality++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Thread.Sleep(10);

                        size = CompressionTests.Brotli(quality, window, ref span);
                    }

                    span = new TimeSpan(0);

                    for (int i = 0; i < 7; i++)
                    {
                        Thread.Sleep(10);

                        size = CompressionTests.Brotli(quality, window, ref span);
                    }

                    span /= 7;

                    resultTable[window - 10, quality] = new TestResult(size, span);

                    if (size < results[CompressionLevel.Fastest].Size)
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkRed;

                    PrintRight(size.ToString());
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.ReadKey(true);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"  Brotli compressed size compared to Deflate Optimal in Bytes: {uncompressedSize} -> {results[CompressionLevel.Optimal].Size}.\n");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;

            PrintCenter("Win\\Qual");

            for (int quality = 0; quality <= 11; quality++)
                PrintCenter(quality.ToString());

            for (int window = 10; window <= 24; window++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                PrintRight((Math.Pow(2, window) - 16).ToString());

                Console.ForegroundColor = ConsoleColor.White;

                for (int quality = 0; quality <= 11; quality++)
                {
                    if (resultTable[window - 10, quality].Size < results[CompressionLevel.Optimal].Size)
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkRed;

                    PrintRight(resultTable[window - 10, quality].Size.ToString());
                }
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.ReadKey(true);

            double borderOptimal = uncompressedSize / results[CompressionLevel.Optimal].Span.TotalSeconds / 131072.0;
            double borderFastest = uncompressedSize / results[CompressionLevel.Fastest].Span.TotalSeconds / 131072.0;
            double current;

            Console.WriteLine();
            Console.WriteLine();
            Console.Write($"  Speed in MBit/s: Deflate.Fastest: {borderFastest.ToString("F")}; Deflate.Optimal: {borderOptimal.ToString("F")}.  ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(" > ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" Fastest ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.Write(" > ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" Optimal ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(" > ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(".\n");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;

            PrintCenter("Win\\Qual");

            for (int quality = 0; quality <= 11; quality++)
                PrintCenter(quality.ToString());

            for (int window = 10; window <= 24; window++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                PrintRight((Math.Pow(2, window) - 16).ToString());

                Console.ForegroundColor = ConsoleColor.White;

                for (int quality = 0; quality <= 11; quality++)
                {
                    current = uncompressedSize / resultTable[window - 10, quality].Span.TotalSeconds / 131072.0;

                    if (current < borderOptimal)
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    else if (current < borderFastest)
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkGreen;

                    PrintRight(current.ToString("F"));
                }
            }

            Console.ReadKey(true);

            foreach (double lineSpeed in new double[] { /*ISDN:*/ 0.0625, /*Original ADSL:*/ 1, /*10 MBit:*/ 10, /*100 MBit:*/ 100, /*1 GBit:*/ 1000 })
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;

                borderOptimal = TransmissionTime(results[CompressionLevel.Optimal], lineSpeed).TotalSeconds;
                borderFastest = TransmissionTime(results[CompressionLevel.Fastest], lineSpeed).TotalSeconds;

                Console.WriteLine();
                Console.WriteLine();
                Console.Write($"  Linespeed {lineSpeed} MBit/s in s: Fastest: {borderFastest.ToString("F")}; Optimal: {borderOptimal.ToString("F")}. Legend:  ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(" ## ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Brotli ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write(" ## ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Fastest ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write(" ## ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(" Optimal.\n");

                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                PrintCenter("Win\\Qual");

                for (int quality = 0; quality <= 11; quality++)
                    PrintCenter(quality.ToString());

                for (int window = 10; window <= 24; window++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;

                    PrintRight((Math.Pow(2, window) - 16).ToString());

                    Console.ForegroundColor = ConsoleColor.White;

                    for (int quality = 0; quality <= 11; quality++)
                    {
                        current = TransmissionTime(resultTable[window - 10, quality], lineSpeed).TotalSeconds;

                        if (current < borderOptimal && current < borderFastest)
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                        else if (borderOptimal < borderFastest)
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        else
                            Console.BackgroundColor = ConsoleColor.DarkBlue;

                        PrintRight(current.ToString("F"));
                    }
                }

                Console.ReadKey(true);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine();
        }

        static void PrintCenter(string text)
        {
            if (text.Length > 9)
            {
                Console.Write(text);

                return;
            }

            Console.Write(text.PadLeft(text.Length / 2 + 5).PadRight(10));
        }

        static void PrintRight(string text)
        {
            if (text.Length > 9)
            {
                Console.Write(text);

                return;
            }

            Console.Write(text.PadLeft(9) + " ");
        }

        static TimeSpan TransmissionTime(TestResult result, double mbit)
        {
            double compressedMbit = result.Size / result.Span.TotalSeconds / 131072.0;

            if (compressedMbit < mbit)
                return result.Span;

            return new TimeSpan((long)(result.Size / mbit / 0.0131072));
        }
    }
}
