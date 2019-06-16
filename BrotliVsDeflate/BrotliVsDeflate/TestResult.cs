using System;
using System.Collections.Generic;
using System.Text;

namespace BrotliVsDeflate
{
    struct TestResult
    {
        public int Size;
        public TimeSpan Span;

        public TestResult(int size, TimeSpan span)
        {
            Size = size;
            Span = span;
        }
    }
}
