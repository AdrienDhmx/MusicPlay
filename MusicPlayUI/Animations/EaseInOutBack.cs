using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace MusicPlayUI.Animations
{
    public class EaseInOutBack : EasingFunctionBase
    {
        protected override Freezable CreateInstanceCore()
        {
           return new EaseInOutBack();
        }

        protected override double EaseInCore(double normalizedTime)
        {
            return Math.Pow(normalizedTime, 2.5) - normalizedTime * 0.1 * Math.Sin(normalizedTime * Math.PI);
        }
    }
}
