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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfFinesse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            //txtNumberScreen.Text += b.Content.ToString();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //if (txtNumberScreen.Text.Length > 0)
            //{
            //    txtNumberScreen.Text.Remove(txtNumberScreen.Text.Length - 1, 1);
            //    txtNumberScreen.Text = txtNumberScreen.Text.Remove(txtNumberScreen.Text.Length - 1, 1);
            //}
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {

            //string number = txtNumberScreen.Text;
            //if (number.Length > 1)
            //{

            //}
            //MessageBox.Show(number);
        }
    }
}
