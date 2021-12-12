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

namespace WpfFinesse.TextBoxTrigger
{
    /// <summary>
    /// Interaction logic for TextboxTriggerEX.xaml
    /// </summary>
    public partial class TextboxTriggerEX : Window
    {
        public TextboxTriggerEX()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello1");

        }
    }
}
