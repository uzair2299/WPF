using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfFinesse.ControlUtility
{
    public static class DynamicControlUtility
    {
        public static Expander GetExpenderPhoneBook(string Header)
        {
            Expander Exp = new Expander();
            Grid grid = new Grid();
            grid.ColumnDefinitions.Clear();

            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(70, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(30, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);

            //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            
            TextBlock tb = new TextBlock();
            tb.FontWeight = FontWeights.SemiBold;
            tb.FontSize = 14;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Text = "Uzair Anwar " + Header;

            StackPanel callControlsStackPanel = new StackPanel();
            callControlsStackPanel.Orientation = Orientation.Horizontal;
            callControlsStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            Button bn = new Button();
            bn.Cursor = Cursors.Hand;
            bn.BorderBrush = Brushes.Transparent;
            bn.Width = 28;
            bn.Height = 28;
            bn.Margin = new Thickness(1);
            bn.BorderThickness = new Thickness(0);
            bn.Content = new Image
            {
                Source = new BitmapImage(new Uri("/Resources/call-end.png", UriKind.RelativeOrAbsolute)),
                VerticalAlignment = VerticalAlignment.Center,
                Width=25,
                Height=25
            };


            Button bn1 = new Button();
            bn1.BorderBrush = Brushes.Transparent;
            bn1.Cursor = Cursors.Hand;
            bn1.Width = 28;
            bn1.Height = 28;
            bn1.BorderThickness = new Thickness(0);
            bn1.Content = new Image
            {
                Source = new BitmapImage(new Uri("/Resources/call-answer.png", UriKind.RelativeOrAbsolute)),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 25,
                Height = 25
            };

            callControlsStackPanel.Children.Add(bn);
            callControlsStackPanel.Children.Add(bn1);


            grid.Children.Add(tb);
            grid.Children.Add(callControlsStackPanel);

            Grid.SetColumn(callControlsStackPanel, 1);
            Grid.SetColumn(tb, 0);


            Exp.Header = grid;
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
