using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DynamicScrollViewer
{
    public class OnScrollEvent(double delta, double verticalOffset, double horizontalOffset, bool isScrollingVertically, bool isScrollingForward, bool scrollInitiatedByAnimation, DynamicScrollViewer sender)
    {
        public double VerticalOffset { get; private set; } = verticalOffset;
        public double HorizontalOffset { get; private set; } = horizontalOffset;

        public double Delta { get; private set; } = delta;
        /// <summary>
        /// determines if the scroll is vertical or horizontal
        /// </summary>
        public bool IsScrollingVertically { get; private set; } = isScrollingVertically;

        /// <summary>
        /// determines if the scroll is in the direction is down for vertical Scrolling or to the right for horizontal scrolling
        /// </summary>
        public bool IsScrollingForward { get; private set; } = isScrollingForward;

        public bool ScrollInitiatedByAnimation { get; private set; } = scrollInitiatedByAnimation;

        public DynamicScrollViewer Sender { get; private set; } = sender;
    }
}
