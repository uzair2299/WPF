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
    /// Interaction logic for Tabs.xaml
    /// </summary>
    public partial class Tabs : Window
    {
        public Tabs()
        {
            InitializeComponent();
            List<UserTab> users = new List<UserTab>();
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John", LastName = "John Doe", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() {  CallStatus = "Hold", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John uzair Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });

            //UserTab u = new UserTab() { Id = 3, FirrstName = "Uzair", LastName = "Anwar", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) };
            //dgSimple.Items.Add(u);
            //if (u.FirrstName == "Uzair")
            //{

            //}
            dgSimple.ItemsSource = users;
            dgSimple1.ItemsSource = users;
            SearchTermTextBox.Text = "Enter text here...";

            SearchTermTextBox.GotFocus += RemoveText;
            SearchTermTextBox.LostFocus += AddText;

        }

        private void SearchTermTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnCallVaribles_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string name = b.Name;
            if (name == "btnConsultCallVaribles")
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
            else if (name == "btnCallVaribles")
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

        public void RemoveText(object sender, EventArgs e)
        {
            if (SearchTermTextBox.Text == "Enter text here...")
            {
                SearchTermTextBox.Text = "";
            }
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTermTextBox.Text))
                SearchTermTextBox.Text = "Enter text here...";
        }

    }

    public class UserTab
    {
        public int Id { get; set; }

        public string FirrstName { get; set; }

        public string LastName { get; set; }
        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string RowDetail { get; set; }

        public int ActiveParticipants { get; set; }
        public int HeldParticipants { get; set; }
        public int Duration { get; set; }
        public string CallStatus { get; set; }
        public string QueueName { get; set; }
    }
}
