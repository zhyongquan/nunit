// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    partial class ValueGenerator
    {
        private sealed class Int64ValueGenerator : ValueGenerator<long>
        {
            public override IEnumerable<long> GenerateRange(long start, long end, Step step)
            {
                if (start == end)
                {
                    yield return start;
                }
                else if ((start < end && !step.IsPositive) || (end < start && !step.IsNegative))
                {
                    throw new ArgumentException("Step must be in the direction of the end.");
                }
                else
                {
                    for (var current = start;;)
                    {
                        yield return current;

                        var next = step.Apply(current);

                        if (start < end)
                        {
                            if (end < next) break; // We stepped past the end of the range.
                            if (next < current) break; // We overflowed which means we tried to step past the end.
                        }
                        else
                        {
                            if (next < end) break; // We stepped past the end of the range.
                            if (current < next) break; // We overflowed which means we tried to step past the end.
                        }

                        current = next;
                    }
                }
            }

            public override bool TryCreateStep(object value, out ValueGenerator.Step step)
            {
                if (value is long)
                {
                    step = new ComparableStep<long>((long)value, (prev, stepValue) => unchecked(prev + stepValue));
                    return true;
                }

                return base.TryCreateStep(value, out step);
            }
        }
    }
}