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

namespace WpfFinesse
{
    /// <summary>
    /// Interaction logic for Consultcall.xaml
    /// </summary>
    public partial class Consultcall : Window
    {
        public Consultcall()
        {
            InitializeComponent();
        }

        private void btnCallVaribles_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
           string name =  b.Name;
            if(name== "btnConsultCallVaribles")
            {
                if (ConsultCallVariablePanel.Visibility == Visibility.Visible)
                {
                    ConsultCallVariablePanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ConsultCallVariablePanel.Visibility = Visibility.Visible;
                }
            }
            else if(name== "btnCallVaribles")
            {
                if (CallVariablePanel.Visibility == Visibility.Visible)
                {
                    CallVariablePanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CallVariablePanel.Visibility = Visibility.Visible;
                }
            }
            

        }

    }
}
