using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualizingControls
{
    /// <summary>
    /// Items range.
    /// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
    /// </summary>
    public struct ItemRange
    {
        public int StartIndex { get; }
        public int EndIndex { get; }

        public ItemRange(int startIndex, int endIndex) : this()
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public bool Contains(int itemIndex)
        {
            return itemIndex >= StartIndex && itemIndex <= EndIndex;
        }
    }
}
