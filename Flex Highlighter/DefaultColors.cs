using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Flex_Highlighter
{
    internal class DefaultColors
    {
        internal static Color RegexGroupBackground = Color.FromRgb((byte)(Colors.LightGreen.R / 3), (byte)(Colors.LightGreen.G / 3), (byte)(Colors.LightGreen.B / 3));
        internal static Color RegexCharacterSetBackground = Color.FromRgb(60, 60, 40);
        internal static Color SpecialCharacterForeground = Colors.LightSkyBlue;
        internal static Color EscapedCharacterForeground = Color.FromRgb(252, 56, 7);
    }
}
