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

        //private void btnCallVaribles_Click(object sender, RoutedEventArgs e)
        //{
        //    Button b = (Button)sender;
        //    string name = b.Name;
        //    if (name == "btnConsultCallVaribles")
        //    {
        //        if (ConsultCallVariablePanel.Visibility == Visibility.Visible)
        //        {
        //            ConsultCallVariablePanel.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            ConsultCallVariablePanel.Visibility = Visibility.Visible;
        //        }
        //    }
        //    else if (name == "btnCallVaribles")
        //    {
        //        if (CallVariablePanel.Visibility == Visibility.Visible)
        //        {
        //            CallVariablePanel.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            CallVariablePanel.Visibility = Visibility.Visible;
        //        }
        //    }
        //}

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
                    dgSimple.ItemsSource = u.Where(x => x.AgentName.ToLower().Contains(searchtext));
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

        public string AgentName { get; set; }

        public string Extension { get; set; }
        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string RowDetail { get; set; }

        public int ActiveParticipants { get; set; }
        public int HeldParticipants { get; set; }
        public int Duration { get; set; }
        public string CallStatus { get; set; }
        public string QueueName { get; set; }
        public string Time { get; set; }
        public string CallStatusColor { get; set; }
        public bool isRowDetail { get; set; }

        public List<UserTab> GetUsers()
        {
            List<UserTab> users = new List<UserTab>();
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:40:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = "00:05:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = "00:00:30", QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = "00:06:00", QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = "00:05:00", QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = "00:00:55", QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = "00:00:40", QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = "00:55:00", QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:06:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:10:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:00:20", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:00:41", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:25:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:00:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:00:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:00:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = "00:00:00", QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            return users;
        }
    }

}
