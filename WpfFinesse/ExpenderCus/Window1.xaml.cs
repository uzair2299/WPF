using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfFinesse.ExpenderCus
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Expander ex = new Expander();
            ex.Header = "hello";
            StackPanel Mainst = new StackPanel();

            Border b = new Border();
            b.CornerRadius = new CornerRadius(5);
            b.Margin = new Thickness(5);
            b.Padding = new Thickness(5);
            b.BorderBrush = Brushes.Gray;
            b.BorderThickness = new Thickness(2);

            StackPanel st = new StackPanel();
            TextBlock tb = new TextBlock();
            tb.Text = "Phone NO";
            TextBox txt = new TextBox();
            //txt.Background = Brushes.Red;
            txt.BorderBrush = Brushes.Transparent;
            txt.BorderThickness = new Thickness(0);

            txt.Text = "03336310000";
            st.Children.Add(tb);
            st.Children.Add(txt);
            b.Child = st;





            Border b1 = new Border();
            b1.CornerRadius = new CornerRadius(5);
            b1.Margin = new Thickness(5);
            b1.Padding = new Thickness(5);
            b1.BorderBrush = Brushes.Gray;
            b1.BorderThickness = new Thickness(2);

            StackPanel st1 = new StackPanel();
            TextBlock tb1 = new TextBlock();
            tb1.Text = "Notes";
            TextBox txt1 = new TextBox();
            
            //txt1.Background = Brushes.Red;
            txt1.BorderBrush = Brushes.Transparent;
            txt1.BorderThickness = new Thickness(0);

            txt1.Text = "";
            st1.Children.Add(tb1);
            st1.Children.Add(txt1);
            b1.Child = st1;


            Mainst.Children.Add(b);
            Mainst.Children.Add(b1);




            ex.Content = Mainst;
            main.Children.Add(ex);

        }
    }
}
