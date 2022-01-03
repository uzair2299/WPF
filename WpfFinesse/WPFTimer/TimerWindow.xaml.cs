using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfFinesse.ControlUtility;
using WpfFinesse.CustomTab;

namespace WpfFinesse.WPFTimer
{
    /// <summary>
    /// Interaction logic for TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window
    {
        private List<UserTab> us;
        UserTab user = new UserTab();

        private List<QueueStat> qs;
        QueueStat Que = new QueueStat();
        public TimerWindow()
        {
            InitializeComponent();

            qs = new List<QueueStat>();
            qs = Que.getQueueStat();
            dgSimple1.ItemsSource = qs;


            us = user.GetUsers();
            int totalItem = us.Count();
            int page_ = 1;
            Pager pg = new Pager(totalItem, page_);
            dgSimple.ItemsSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize);
            if (pg.EndPage > 1)
            {
                if (pg.CurrentPage > 1)
                {
                    Button First = new Button();
                    First.Name = "FirstPage";
                    First.Click += Btn_Click;
                    First.Content = "<<";
                    First.Tag = 1;
                    First.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(First);

                    Button Previous = new Button();
                    Previous.Click += Btn_Click;
                    Previous.Content = "<;";
                    Previous.Tag = pg.CurrentPage - 1;
                    Previous.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(Previous);
                }
                for (var page = pg.StartPage; page <= pg.EndPage; page++)
                {
                    Button btn = new Button();
                    btn.Click += Btn_Click;
                    btn.Content = page;
                    btn.Tag = page;
                    btn.Style = (Style)FindResource("btnPagination");
                    if (page == 1)
                    {

                        btn.FontWeight = FontWeights.UltraBold;
                    }
                    StackPagination.Children.Add(btn);

                }
                if (pg.CurrentPage < pg.TotalPages)
                {
                    Button Next = new Button();
                    Next.Click += Btn_Click;
                    Next.Content = ">";
                    Next.Tag = pg.CurrentPage + 1;
                    Next.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(Next);

                    Button Last = new Button();
                    Last.Click += Btn_Click;
                    Last.Content = ">>";
                    Last.Tag = pg.TotalPages;
                    Last.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(Last);
                }
            }
            this.StartMockValue();
            this.StartRefreshDataGrid();




            //phone book
            /* main stack for holding content of expender control*/


            for (var i = 0; i <= 20; i++)
            {

                StackPanel mainPhoneBookStackPanel = new StackPanel();
                Expander expender = DynamicControlUtility.GetExpenderPhoneBook("Jhon Smith " + i);
                Border b = DynamicControlUtility.GetBorderPhoneBook("#C8C6C6", 5, 5, 5, 1);
                
                StackPanel phoneNoStackPanel = new StackPanel();
                TextBlock tb = DynamicControlUtility.GetTextBlockPhoneBook("Phone No");

                TextBox txt = DynamicControlUtility.GetTextBoxPhoneBook("+92320550000"+i);
                phoneNoStackPanel.Children.Add(tb);
                phoneNoStackPanel.Children.Add(txt);
                b.Child = phoneNoStackPanel;


                //notes penal
                Border b1 = DynamicControlUtility.GetBorderPhoneBook("#C8C6C6", 5, 5, 5, 1);

                StackPanel notesStackPanel = new StackPanel();
                TextBlock tb1 = DynamicControlUtility.GetTextBlockPhoneBook("Notes");

                TextBox txt1 = DynamicControlUtility.GetTextBoxPhoneBook("Note "+ i);
                notesStackPanel.Children.Add(tb1);
                notesStackPanel.Children.Add(txt1);
                b1.Child = notesStackPanel;

                //apend phone nd notes into main stackpanel
                mainPhoneBookStackPanel.Children.Add(b);
                mainPhoneBookStackPanel.Children.Add(b1);

                //set expendercontent
                expender.Content = mainPhoneBookStackPanel;

                //set expender into ui stackpanel
                PhoneBookStackPanel.Children.Add(expender);
            }


        }

        private void StartMockValue()
        {
            // NOTE: this is a thread to mock the business logic, it change 'Time' value.
            Task.Run(() =>
            {
                var rando = new Random();
                while (true)
                {
                    foreach (var user in this.us)
                    {
                        user.Time = user.Time + 1;
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void StartRefreshDataGrid()
        {
            // NOTE: this is a thread to update data grid.
            Task.Run(() =>
            {
                while (true)
                {
                    foreach (var user in this.us)
                    {
                        // NOTE: update if the time changed.
                        if (user.DisplayTime != user.Time)
                        {
                            user.DisplayTime = user.Time;
                        }
                    }

                    // NOTE: refresh the grid every seconds.
                    Thread.Sleep(1000);
                }
            });
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;

            if (!string.IsNullOrEmpty(SearchTermTextBox.Text.ToLower()))
            {
                StackPagination.Children.Clear();
                int totalItem = 0;

                us = user.GetUsers();
                string searchtext = SearchTermTextBox.Text.ToLower();
                if (string.IsNullOrEmpty(searchtext))
                {
                    //dgSimple.ItemsSource = u;
                    totalItem = us.Count();

                }
                else
                {
                    us = us.Where(x => x.AgentName.ToLower().Contains(searchtext) || x.CallStatus.ToLower().Contains(searchtext) || x.Time.ToString().ToLower().Contains(searchtext) || x.Extension.ToLower().Contains(searchtext)).ToList();
                    totalItem = us.Count();
                    // dgSimple.ItemsSource = u.Where(x => x.AgentName.ToLower().Contains(searchtext));
                }



                int page_ = Convert.ToInt32(bt.Tag);
                Pager pg = new Pager(totalItem, page_);
                dgSimple.ItemsSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize);
                if (pg.EndPage > 1)
                {
                    if (pg.CurrentPage > 1)
                    {
                        Button First = new Button();
                        First.Name = "FirstPage";
                        First.Click += Btn_Click;
                        First.Content = "<<";
                        First.Tag = 1;
                        First.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(First);

                        Button Previous = new Button();
                        Previous.Click += Btn_Click;
                        Previous.Content = "<";
                        Previous.Tag = pg.CurrentPage - 1;
                        Previous.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(Previous);
                    }
                    for (var page = pg.StartPage; page <= pg.EndPage; page++)
                    {
                        Button btn = new Button();
                        btn.Click += Btn_Click;
                        btn.Content = page;
                        btn.Tag = page;
                        btn.Style = (Style)FindResource("btnPagination");
                        if (page == page_)
                        {

                            btn.FontWeight = FontWeights.UltraBold;
                        }
                        StackPagination.Children.Add(btn);

                    }
                    if (pg.CurrentPage < pg.TotalPages)
                    {
                        Button Next = new Button();
                        Next.Click += Btn_Click;
                        Next.Content = ">";
                        Next.Tag = pg.CurrentPage + 1;
                        Next.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(Next);

                        Button Last = new Button();
                        Last.Click += Btn_Click;
                        Last.Content = ">>";
                        Last.Tag = pg.TotalPages;
                        Last.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(Last);
                    }
                }
            }
            else
            {
                //MessageBox.Show(bt.Tag.ToString());
                StackPagination.Children.Clear();


                us = user.GetUsers();

                int totalItem = us.Count();
                int page_ = Convert.ToInt32(bt.Tag);
                Pager pg = new Pager(totalItem, page_);
                dgSimple.ItemsSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize);
                if (pg.EndPage > 1)
                {
                    if (pg.CurrentPage > 1)
                    {
                        Button First = new Button();
                        First.Name = "FirstPage";
                        First.Click += Btn_Click;
                        First.Content = "<<";
                        First.Tag = 1;
                        First.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(First);

                        Button Previous = new Button();
                        Previous.Click += Btn_Click;
                        Previous.Content = "<";
                        Previous.Tag = pg.CurrentPage - 1;
                        Previous.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(Previous);
                    }
                    for (var page = pg.StartPage; page <= pg.EndPage; page++)
                    {
                        Button btn = new Button();
                        btn.Click += Btn_Click;
                        btn.Content = page;
                        btn.Tag = page;
                        btn.Style = (Style)FindResource("btnPagination");
                        if (page == page_)
                        {

                            btn.FontWeight = FontWeights.UltraBold;
                        }
                        StackPagination.Children.Add(btn);

                    }
                    if (pg.CurrentPage < pg.TotalPages)
                    {
                        Button Next = new Button();
                        Next.Click += Btn_Click;
                        Next.Content = ">";
                        Next.Tag = pg.CurrentPage + 1;
                        Next.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(Next);

                        Button Last = new Button();
                        Last.Click += Btn_Click;
                        Last.Content = ">>";
                        Last.Tag = pg.TotalPages;
                        Last.Style = (Style)FindResource("btnPagination");
                        StackPagination.Children.Add(Last);
                    }
                }

            }
        }


        //private void btnCallVaribles_Click(object sender, RoutedEventArgs e)
        //{
        //    Button b = (Button)sender;
        //    string name = b.Name;
        //    if (name == "btnConsultCallVaribles")
        //    {
        //        //if (ConsultCallVariablePanel.Visibility == Visibility.Visible)
        //        //{
        //        //    ConsultCallVariablePanel.Visibility = Visibility.Collapsed;
        //        //}
        //        //else
        //        //{
        //        //    ConsultCallVariablePanel.Visibility = Visibility.Visible;
        //        //}
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


        private void SearchTermTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            StackPagination.Children.Clear();
            int totalItem = 0;
            us = user.GetUsers();
            string searchtext = SearchTermTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(searchtext))
            {
                //dgSimple.ItemsSource = u;
                totalItem = us.Count();
                StackNoData.Visibility = Visibility.Collapsed;
                dgSimple.Visibility = Visibility.Visible;

            }
            else
            {
                us = us.Where(x => x.AgentName.ToLower().Contains(searchtext) || x.CallStatus.ToLower().Contains(searchtext) || x.Time.ToString().ToLower().Contains(searchtext) || x.Extension.ToLower().Contains(searchtext)).ToList();
                totalItem = us.Count();
                if (totalItem == 0)
                {
                    StackNoData.Visibility = Visibility.Visible;
                    dgSimple.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StackNoData.Visibility = Visibility.Collapsed;
                    dgSimple.Visibility = Visibility.Visible;
                }
                // dgSimple.ItemsSource = u.Where(x => x.AgentName.ToLower().Contains(searchtext));
            }




            Pager pg = new Pager(totalItem, 1);
            dgSimple.ItemsSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize);
            if (pg.EndPage > 1)
            {
                if (pg.CurrentPage > 1)
                {
                    Button First = new Button();
                    First.Name = "FirstPage";
                    First.Click += Btn_Click;
                    First.Content = "<<";
                    First.Tag = 1;
                    First.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(First);

                    Button Previous = new Button();
                    Previous.Click += Btn_Click;
                    Previous.Content = "<";
                    Previous.Tag = pg.CurrentPage - 1;
                    Previous.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(Previous);
                }
                for (var page = pg.StartPage; page <= pg.EndPage; page++)
                {
                    Button btn = new Button();
                    btn.Click += Btn_Click;
                    btn.Content = page;
                    btn.Tag = page;
                    btn.Style = (Style)FindResource("btnPagination");
                    if (page == 1)
                    {

                        btn.FontWeight = FontWeights.UltraBold;
                    }
                    StackPagination.Children.Add(btn);

                }
                if (pg.CurrentPage < pg.TotalPages)
                {
                    Button Next = new Button();
                    Next.Click += Btn_Click;
                    Next.Content = ">";
                    Next.Tag = pg.CurrentPage + 1;
                    Next.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(Next);

                    Button Last = new Button();
                    Last.Click += Btn_Click;
                    Last.Content = ">>";
                    Last.Tag = pg.TotalPages;
                    Last.Style = (Style)FindResource("btnPagination");
                    StackPagination.Children.Add(Last);
                }
            }








            //if (e.Key == Key.Return)
            //{
            //    dgSimple.ItemsSource = u.Where(x => x.FirrstName.Contains(SearchTermTextBox.Text));
            //}
            //else if (e.Key == Key.Back)
            //{
            //    dgSimple.ItemsSource = u.Where(x => x.FirrstName.Contains(SearchTermTextBox.Text));
            //}



            //if (Keyboard.IsKeyDown(Key.Return)) //Also "Key.Delete" is available.
            //{
            //    string searchtext = SearchTermTextBox.Text.ToLower();
            //    if (string.IsNullOrEmpty(searchtext))
            //    {
            //        dgSimple.ItemsSource = u;
            //    }
            //    else
            //    {
            //        dgSimple.ItemsSource = u.Where(x => x.AgentName.ToLower().Contains(searchtext));
            //    }

            //}



            //else if (Keyboard.IsKeyDown(Key.Back)) //Also "Key.Delete" is available.
            //{
            //    dgSimple.ItemsSource = u.Where(x => x.FirrstName.Contains(SearchTermTextBox.Text));
            //}


        }

        private void btnChangeTimer_Click(object sender, RoutedEventArgs e)
        {
            string input = TxtUserInput.Text;
            TxtUserInput.Text = "";
            foreach (var mc in this.us)
            {
                if (mc.AgentName.ToLower() == input.ToLower())
                {
                    mc.Time = 0;
                }
            }
        }
    }

    public class UserTab : INotifyPropertyChanged
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
        public string CallStatusColor { get; set; }
        public bool isRowDetail { get; set; }


        // NOTE: this is the 'real time', some business thread will update the value.
        public int Time { get; set; }

        private int displayTime;

        // NOTE: this is the 'displayed time' in the DataGrid.
        public int DisplayTime
        {
            get { return displayTime; }
            set
            {
                displayTime = value;
                this.OnPropertyChange(nameof(this.DisplayTimeString));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public string DisplayTimeString => $"{(displayTime / 60 / 60).ToString().PadLeft(2, '0')}:{(displayTime / 60 % 60).ToString().PadLeft(2, '0')}:{(displayTime % 60).ToString().PadLeft(2, '0')}";


        public List<UserTab> GetUsers()
        {
            List<UserTab> users = new List<UserTab>();
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 100, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 210, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 220, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 230, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 240, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 260, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 270, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 280, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 290, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 310, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 320, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 100, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 110, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 280, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 450, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 200, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 300, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 300, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 300, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 300, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 300, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 300, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 300, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 400, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 400, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 400, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 400, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 400, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 400, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 400, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 500, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 500, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 500, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 500, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 500, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 500, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 500, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });

            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 600, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 600, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 800, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 800, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 800, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 800, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 800, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 800, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 800, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 850, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 850, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 850, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 850, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 850, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 850, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 850, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 900, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 900, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 900, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 900, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 900, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 900, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 900, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });

            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "John Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "AJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "BJohn Doe", Extension = "42037", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "CJohn Doe", Extension = "42038", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "DJohn Doe", Extension = "42039", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 250, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "EJohn Doe", Extension = "42003", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "EJohn Doe", Extension = "42053", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "FJohn Doe", Extension = "42073", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "GJohn", Extension = "42078", Name = "Sammy", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "HJohn Doe", Extension = "42083", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "IJohn Doe", Extension = "42093", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "JJohn Doe", Extension = "42013", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "KJohn Doe", Extension = "42000", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "LJohn Doe", Extension = "47033", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 250, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John uzair Doe", Extension = "74441", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });

            return users;
        }
    }



    public class QueueStat
    {
        public string QueueName { get; set; }
        public int QueuedCalls { get; set; }

        public string MaxTime { get; set; }
        public int Ready { get; set; }
        public int NotReady { get; set; }
        public int TalkingIN { get; set; }
        public int TalkingOUT { get; set; }
        public int TalkingINT { get; set; }

        public List<QueueStat> getQueueStat()
        {
            List<QueueStat> list = new List<QueueStat>();
            list.Add(new QueueStat { QueueName = "Marketing", QueuedCalls = 60, MaxTime = "01:36:25", Ready = 10, NotReady = 5, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "Inbound", QueuedCalls = 6, MaxTime = "01:03:25", Ready = 6, NotReady = 10, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "OutBound", QueuedCalls = 600, MaxTime = "00:36:25", Ready = 100, NotReady = 52, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "Product Support", QueuedCalls = 6, MaxTime = "00:45:10", Ready = 6, NotReady = 100, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "Product Information", QueuedCalls = 6, MaxTime = "02:36:25", Ready = 6, NotReady = 10, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "Channel Marketing", QueuedCalls = 6, MaxTime = "01:36:25", Ready = 15, NotReady = 10, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "Technical Support", QueuedCalls = 6, MaxTime = "01:36:25", Ready = 6, NotReady = 2, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            list.Add(new QueueStat { QueueName = "Vip Technical Support", QueuedCalls = 6, MaxTime = "01:36:25", Ready = 6, NotReady = 10, TalkingIN = 10, TalkingOUT = 10, TalkingINT = 10 });
            return list;
        }
    }


    public class ToolTipCon : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString().Length > 15)
            {
                return Visibility.Visible;
            }
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];

                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}