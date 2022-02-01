using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfFinesse.ControlUtility
{
    public static class DynamicControlUtility
    {
        public static Expander GetExpenderPhoneBook(string Header, string phoneNumber = null)
        {
            Expander Exp = new Expander();
            Grid grid = new Grid();
            grid.ColumnDefinitions.Clear();
            grid.Height = 28.5;

            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(65, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(35, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);

            TextBlock tb = new TextBlock();
            tb.FontWeight = FontWeights.SemiBold;
            tb.FontSize = 14;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Text = Header;
            if (Header.Length > 30)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.Style = (Style)Application.Current.FindResource("PhoneBookToolTip");
                tooltip.Placement = PlacementMode.Bottom;
                //tooltip.PlacementRectangle = new Rect(20, 0, 0, 0);
                tooltip.HorizontalOffset = 0;
                tooltip.VerticalOffset = 0;
                tooltip.Content = Header;
                tb.ToolTip = tooltip;
                //tb.TextWrapping = TextWrapping.Wrap;
                tb.TextTrimming = TextTrimming.CharacterEllipsis;
            }
            StackPanel callControlsStackPanel = new StackPanel();
            callControlsStackPanel.Orientation = Orientation.Horizontal;
            callControlsStackPanel.HorizontalAlignment = HorizontalAlignment.Right;
            //Button bn = new Button();
            //bn.Cursor = Cursors.Hand;
            //bn.BorderBrush = Brushes.Transparent;
            //bn.Width = 28;
            //bn.Height = 28;
            //bn.Click += Bn_Click;
            //bn.Margin = new Thickness(1);
            //bn.BorderThickness = new Thickness(0);
            //bn.Content = new Image
            //{
            //    Source = new BitmapImage(new Uri("/Resources/call-end.png", UriKind.RelativeOrAbsolute)),
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Width=25,
            //    Height=25
            //};


            Button bn1 = new Button();
            bn1.BorderBrush = Brushes.Transparent;
            bn1.Cursor = Cursors.Hand;
            bn1.Name = "MakeCall";
            bn1.Visibility = Visibility.Collapsed;
            bn1.Width = 28;
            bn1.Height = 28;
            bn1.Tag = phoneNumber;
            bn1.Click += Bn_Click;
            bn1.BorderThickness = new Thickness(0);
            bn1.Content = new Image
            {
                Source = new BitmapImage(new Uri("/Resources/call-answer.png", UriKind.RelativeOrAbsolute)),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 25,
                Height = 25
            };


            //Button bn3 = new Button();
            //bn3.BorderBrush = Brushes.Transparent;
            //bn3.Cursor = Cursors.Hand;
            //bn3.Width = 28;
            //bn3.Height = 28;
            //bn3.Click += Bn_Click;
            //bn3.BorderThickness = new Thickness(0);
            //bn3.Content = new Image
            //{
            //    Source = new BitmapImage(new Uri("/Resources/call-transfer.png", UriKind.RelativeOrAbsolute)),
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Width = 25,
            //    Height = 25
            //};



            //callControlsStackPanel.Children.Add(bn);
            callControlsStackPanel.Children.Add(bn1);
            //callControlsStackPanel.Children.Add(bn3);


            grid.Children.Add(tb);
            grid.Children.Add(callControlsStackPanel);

            Grid.SetColumn(callControlsStackPanel, 1);
            Grid.SetColumn(tb, 0);


            Exp.Header = grid;
            Exp.MouseEnter += Exp_MouseEnter;
            Exp.MouseLeave += Exp_MouseLeave;
            return Exp;
        }

        private static void Exp_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                Expander ex = (Expander)sender;
                Grid gd = (Grid)ex.Header;
                var i = Grid.GetColumn(gd);
                var ii = gd.Children.Cast<UIElement>().Where(x => Grid.GetColumn(x) == 1).FirstOrDefault();
                if (ii != null)
                {
                    StackPanel st = (StackPanel)ii;
                    foreach (var item in st.Children)
                    {
                        if (item is Button)
                        {
                            Button btn = (Button)item;
                            switch (btn.Name)
                            {
                                case "MakeCall":
                                    btn.Visibility = Visibility.Collapsed;
                                    break;
                            }
                        }
                    }
                }




            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static void Exp_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                Expander ex = (Expander)sender;
                Grid gd = (Grid)ex.Header;
                var i = Grid.GetColumn(gd);
                var ii = gd.Children.Cast<UIElement>().Where(x => Grid.GetColumn(x) == 1).FirstOrDefault();
                if (ii != null)
                {
                    StackPanel st = (StackPanel)ii;
                    foreach (var item in st.Children)
                    {
                        if (item is Button)
                        {
                            Button btn = (Button)item;
                            switch (btn.Name)
                            {
                                case "MakeCall":
                                    btn.Visibility = Visibility.Visible;
                                    break;
                            }
                        }
                    }
                }




            }
            catch (Exception ex)
            {

                throw ex;
            }

            //gd.Children.Cast<UIElement>().First(x => Grid.GetRow(x) == row && Grid.GetColumn(x) == Button);
        }

        private static void Bn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            MessageBox.Show("Call - " + btn.Tag.ToString());
        }

        public static Border GetBorderPhoneBook(string BorderColorBrush, int CornerRadius, int Margin, int Padding, int BorderThickness)
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

        public static TextBlock GetTextBoxPhoneBook(string Text)
        {
            TextBlock txt = new TextBlock();
            //txt.BorderBrush = Brushes.Transparent;
            //txt.BorderThickness = new Thickness(0);
            txt.FontWeight = FontWeights.SemiBold;
            //txt.IsReadOnly = true;
            txt.TextWrapping = TextWrapping.Wrap;
            txt.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#938c8c");
            txt.Text = Text;
            return txt;
        }
    }
}
