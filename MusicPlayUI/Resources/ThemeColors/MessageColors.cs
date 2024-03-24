using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicPlayUI.ThemeColors
{
    public static class MessageColors
    {
        public static readonly Brush InfoContainer = CreateBrush("#053C9A");
        public static readonly Brush OnInfoContainer = CreateBrush("#dae2ff");
        public static readonly Brush InfoHover = CreateBrush("#803E6EC3");

        public static readonly Brush SuccessContainer = CreateBrush("#265E0A");
        public static readonly Brush OnSuccessContainer = CreateBrush("#c2f18d");
        public static readonly Brush SuccessHover = CreateBrush("#805EB035");

        public static readonly Brush WarningContainer = CreateBrush("#B97606");
        public static readonly Brush OnWarningContainer = CreateBrush("#ebe1d1");
        public static readonly Brush WarningHover = CreateBrush("#80CB8D24");

        public static readonly Brush ErrorContainer = CreateBrush("#8C1D18");
        public static readonly Brush OnErrorContainer = CreateBrush("#F9DEDC");
        public static readonly Brush ErrorHover = CreateBrush("#60B3261E");

        public static Brush CreateBrush(string hex)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(hex);
        }
    }
}
