using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfFinesse.ControlUtility
{
    public static class DynamicControlUtility
    {
        public static Expander GetExpenderPhoneBook(string Header)
        {
            Expander Exp = new Expander();
            Exp.Header = Header;
            return Exp;
        }

        public static Border GetBorderPhoneBook(string BorderColorBrush, int CornerRadius,int Margin,int Padding,int BorderThickness)
        {
            Border b = new Border();
            b.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom(BorderColorBrush);
            b.CornerRadius = new CornerRadius(CornerRadius);
            b.Margin = new Thickness(Margin);
            b.Padding = new Thickness(Padding);
            b.BorderThickness = new Thickness(BorderThickness);
            return b;
        }

        public static TextBlock GetTextBlockPhoneBook(string Text)
        {
            TextBlock tb = new TextBlock();
            tb.FontWeight = FontWeights.Light;
            tb.FontSize = 12;
            tb.Text = Text;

            return tb;
        }

        public static TextBox GetTextBoxPhoneBook(string Text)
        {
            TextBox txt = new TextBox();
            txt.BorderBrush = Brushes.Transparent;
            txt.BorderThickness = new Thickness(0);
            txt.FontWeight = FontWeights.SemiBold;
            txt.IsReadOnly = true;
            txt.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#938c8c");
            txt.Text = Text;
            return txt;
        }
    }
}
