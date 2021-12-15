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

            //UserTab u = new UserTab() { Id = 3, FirrstName = "Uzair", LastName = "Anwar", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) };
            //dgSimple.Items.Add(u);
            //if (u.FirrstName == "Uzair")
            //{

            //}
            UserTab user = new UserTab();
            dgSimple.ItemsSource = user.GetUsers();
            dgSimple1.ItemsSource = user.GetUsers();
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

        private void SearchTermTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            UserTab user1 = new UserTab();
            List<UserTab> u = new List<UserTab>();
            u = user1.GetUsers();

            //if (e.Key == Key.Return)
            //{
            //    dgSimple.ItemsSource = u.Where(x => x.FirrstName.Contains(SearchTermTextBox.Text));
            //}
            //else if (e.Key == Key.Back)
            //{
            //    dgSimple.ItemsSource = u.Where(x => x.FirrstName.Contains(SearchTermTextBox.Text));
            //}
            if (Keyboard.IsKeyDown(Key.Return)) //Also "Key.Delete" is available.
            {
                string searchtext = SearchTermTextBox.Text.ToLower();
                if (string.IsNullOrEmpty(searchtext))
                {
                    dgSimple.ItemsSource = u;
                }
                else
                {
                    dgSimple.ItemsSource = u.Where(x => x.FirrstName.ToLower().Contains(searchtext));
                }

            }
            //else if (Keyboard.IsKeyDown(Key.Back)) //Also "Key.Delete" is available.
            //{
            //    dgSimple.ItemsSource = u.Where(x => x.FirrstName.Contains(SearchTermTextBox.Text));
            //}


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

        public List<UserTab> GetUsers()
        {
            List<UserTab> users = new List<UserTab>();
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "AJohn Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "BJohn Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "CJohn Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "DJohn Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "EJohn Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "EJohn Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "FJohn Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "GJohn", LastName = "John Doe", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "HJohn Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "IJohn Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "JJohn Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 1, FirrstName = "KJohn Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { QueueName = "AMQ", CallStatus = "Active", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 2, FirrstName = "LJohn Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatus = "Hold", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, RowDetail = "This Is row Details", Id = 3, FirrstName = "John uzair Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            return users;
        }
    }
}
