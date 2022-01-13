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
using WpfFinesse.AMQ;
using WpfFinesse.ControlUtility;
using WpfFinesse.CustomTab;
using WpfFinesse.Models;

namespace WpfFinesse.WPFTimer
{
    /// <summary>
    /// Interaction logic for TimerWindow.xaml
    /// </summary>
    public partial class TimerWindow : Window
    {
        private bool timerOnce = false;
        private List<UserTab> us;
        List<UserTab> teamUserList = new List<UserTab>();
        UserTab user = new UserTab();

        AMQManager aMQManager = AMQManager.GetInstance();
        private List<QueueStat> qs;
        QueueStat Que = new QueueStat();
        public TimerWindow()
        {
            aMQManager = AMQManager.GetInstance();
            aMQManager.InitializeAMQ();
            aMQManager.messageArrived -= AMQManager_messageArrived;
            aMQManager.messageArrived += AMQManager_messageArrived;
            aMQManager.UpdateTopic();
            InitializeComponent();
            try
            {
                List<ComboBoxPair> cb = new List<ComboBoxPair>();
                string[] teams = CallEventInfoListing.Teams.Split('|');

                if (teams.Length == 0)
                {

                }
                else
                {
                    foreach (string item in teams)
                    {
                        cb.Add(new ComboBoxPair(item.Split(',')[0], item.Split(',')[1]));

                    }
                    CTITeams.DisplayMemberPath = "_Value";
                    CTITeams.SelectedValuePath = "_Key";
                    CTITeams.ItemsSource = cb;
                    CTITeams.SelectedIndex = 0;
                }

                ComboBoxPair cbp = (ComboBoxPair)CTITeams.SelectedItem;

                string _key = cbp._Key;
                string _value = cbp._Value;

                aMQManager.SendMessageToQueue(GCMessages("getteamusers"), _key + ",true");
            }
            catch (Exception e)
            {

                throw;
            }

            qs = new List<QueueStat>();
            qs = Que.getQueueStat();
            dgSimple1.ItemsSource = qs;


            us = user.GetUsers();
            int totalItem = us.Count();
            int page_ = 1;
            Pager pg = new Pager(totalItem, page_);
            //dgSimple.ItemsSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize);
            //if (pg.EndPage > 1)
            //{
            //    if (pg.CurrentPage > 1)
            //    {
            //        Button First = new Button();
            //        First.Name = "FirstPage";
            //        First.Click += Btn_Click;
            //        First.Content = "<<";
            //        First.Tag = 1;
            //        First.Style = (Style)FindResource("btnPagination");
            //        StackPagination.Children.Add(First);

            //        Button Previous = new Button();
            //        Previous.Click += Btn_Click;
            //        Previous.Content = "<;";
            //        Previous.Tag = pg.CurrentPage - 1;
            //        Previous.Style = (Style)FindResource("btnPagination");
            //        StackPagination.Children.Add(Previous);
            //    }
            //    for (var page = pg.StartPage; page <= pg.EndPage; page++)
            //    {
            //        Button btn = new Button();
            //        btn.Click += Btn_Click;
            //        btn.Content = page;
            //        btn.Tag = page;
            //        btn.Style = (Style)FindResource("btnPagination");
            //        if (page == 1)
            //        {

            //            btn.FontWeight = FontWeights.UltraBold;
            //        }
            //        StackPagination.Children.Add(btn);

            //    }
            //    if (pg.CurrentPage < pg.TotalPages)
            //    {
            //        Button Next = new Button();
            //        Next.Click += Btn_Click;
            //        Next.Content = ">";
            //        Next.Tag = pg.CurrentPage + 1;
            //        Next.Style = (Style)FindResource("btnPagination");
            //        StackPagination.Children.Add(Next);

            //        Button Last = new Button();
            //        Last.Click += Btn_Click;
            //        Last.Content = ">>";
            //        Last.Tag = pg.TotalPages;
            //        Last.Style = (Style)FindResource("btnPagination");
            //        StackPagination.Children.Add(Last);
            //    }
            //}
            //this.StartMockValue();
            //this.StartRefreshDataGrid();




            //phone book
            /* main stack for holding content of expender control*/


            List<UserTab> phoneBookSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize).ToList();
            SetPhoneBookSource(phoneBookSource);


            if (pg.EndPage > 1)
            {
                if (pg.CurrentPage > 1)
                {
                    Button First = new Button();
                    First.Name = "FirstPage";
                    First.Click += Btn_ClickPhoneBook;
                    First.Content = "<<";
                    First.Tag = 1;
                    First.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(First);

                    Button Previous = new Button();
                    Previous.Click += Btn_ClickPhoneBook;
                    Previous.Content = "<;";
                    Previous.Tag = pg.CurrentPage - 1;
                    Previous.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(Previous);
                }
                for (var page = pg.StartPage; page <= pg.EndPage; page++)
                {
                    Button btn = new Button();
                    btn.Click += Btn_ClickPhoneBook;
                    btn.Content = page;
                    btn.Tag = page;
                    btn.Style = (Style)FindResource("btnPagination");
                    if (page == 1)
                    {

                        btn.FontWeight = FontWeights.UltraBold;
                    }
                    PhoneBookPagination.Children.Add(btn);

                }
                if (pg.CurrentPage < pg.TotalPages)
                {
                    Button Next = new Button();
                    Next.Click += Btn_ClickPhoneBook;
                    Next.Content = ">";
                    Next.Tag = pg.CurrentPage + 1;
                    Next.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(Next);

                    Button Last = new Button();
                    Last.Click += Btn_ClickPhoneBook;
                    Last.Content = ">>";
                    Last.Tag = pg.TotalPages;
                    Last.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(Last);
                }
            }


        }

        private string GCMessages(string cmd)
        {
            cmd = cmd + "#" + CallEventInfoListing.agentID;
            return cmd;
        }

        private string GCMessages(string cmd, string loginId)
        {
            cmd = cmd + "#" + loginId;
            return cmd;
        }

        private void AMQManager_messageArrived(object sender, MyEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        string[] events = args.eventArgs;
                        EventType eventName = EventType.NONE;
                        eventName = GetEnumValue<EventType>(events[1]);

                        switch (eventName)
                        {

                            case EventType.State:
                                string agentstate = events[2].ToUpper();
                                string _AgentRefId = events[0];
                                string agentExtension = events[9];
                                string agentLoginId = events[8];
                                AgentState eAgentState = GetEnumValue<AgentState>(agentstate);
                                switch (eAgentState)
                                {
                                    case AgentState.NOT_READY:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //AgentStateReadyNotReady(CtiAgentStates.UNAVAILABLE);
                                        }
                                        else
                                        {
                                            var CheckAlreadyExist = teamUserList.Where(x => x.LoginId == agentLoginId).FirstOrDefault();
                                            if (CheckAlreadyExist != null)
                                            {
                                                foreach (var item in teamUserList)
                                                {
                                                    if (item.LoginId == events[8])
                                                    {
                                                        item.CallStatus = eAgentState.ToString();
                                                        item.Time = 0;
                                                        item.Extension = agentExtension;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                UserTab agent = new UserTab();
                                                
                                                string agentState = events[2];
                                                agent.CallStatus = agentState;
                                                agent.AgentName = events[7];
                                                agent.LoginId = events[8];
                                                agent.Extension = events[9];
                                                
                                                agent.CallStatusColor = "#1BDA6D";
                                                agent.Time = 0;
                                                //if (agentState != "LOGOUT")
                                                //{
                                                //    string[] t = events[7].Split(new[] { ':' }, 2);

                                                //    string[] time = t[1].Split();
                                                //    //DateTime d = new DateTime((int)time[5], time[1], time[2]);
                                                //    DateTime date = DateTime.Parse(time[3], System.Globalization.CultureInfo.CurrentCulture);

                                                //    DateTime now = DateTime.Now;
                                                //    int diffInSeconds = (int)(now - date).TotalSeconds;
                                                //    agent.Time = diffInSeconds;
                                                //}
                                                teamUserList.Add(agent);

                                            }


                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }
                                        break;
                                    case AgentState.READY:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //AgentStateReadyNotReady(CtiAgentStates.AVAILABLE);
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 0;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }

                                        break;
                                    case AgentState.RESERVED:
                                        //CallEventInfoListing.isLoggedIn = true;
                                        //checkforNotReadyItemsNWrapupReasonLabels();
                                        //CallInfoData callReservedInfoData = new CallInfoData();
                                        //callReservedInfoData.CurrentCallState = Enum.GetName(typeof(CallClassProvider.CallState), CallClassProvider.CallState.Accepted);
                                        //CallStateChangeEvent(this, new CtiCoreEventArgs("CallStateChanged", callReservedInfoData, events[2]));
                                        break;
                                    case AgentState.TALKING:

                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //CallEventInfoListing.isLoggedIn = true;
                                            //checkforNotReadyItemsNWrapupReasonLabels();
                                            //CallInfoData callTalkingInfoData = new CallInfoData();
                                            //callTalkingInfoData.CurrentCallState = Enum.GetName(typeof(CallClassProvider.CallState), CallClassProvider.CallState.Busy);
                                            //CallStateChangeEvent(this, new CtiCoreEventArgs("CallStateChanged", callTalkingInfoData, events[2]));
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 0;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }

                                        break;
                                    case AgentState.WORK_READY:
                                        //AgentStateWrapUp(events[2]);
                                        break;
                                    case AgentState.WORK:
                                        //AgentStateWrapUp(events[2]);
                                        break;
                                    case AgentState.LOGOUT:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //DeactivateConnection();
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Extension = "";
                                                    item.Time = 0;
                                                }
                                            }
                                            teamUserList = teamUserList.Where(x => x.CallStatus != "LOGOUT").ToList();
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }

                                        break;
                                    case AgentState.RE_LOGIN:
                                        //DeactivateConnection();
                                        break;
                                    case AgentState.HOLD:
                                        //CallStateHold(CallEventInfoListing.currentActiveCall);
                                        break;
                                }
                                break;
                            case EventType.AgentInfo:
                                CallEventInfoListing.isWraupAllowed = !events[5].ToUpperInvariant().Equals("NOT_ALLOWED");

                                CallEventInfoListing.agentID = events[0];
                                if (Convert.ToBoolean(events[4]) == true)
                                {
                                    CallEventInfoListing.isSupervisor = Convert.ToBoolean(events[4]);
                                    //string number = events[6].Split(':')[1];
                                    CallEventInfoListing.totalNumberOfTeams = Convert.ToInt32(events[6].Split(':')[1]);
                                    CallEventInfoListing.Teams = events[7];
                                    CallEventInfoListing.agentFullName = events[8];

                                }
                                break;
                            case EventType.TeamUsersList:
                                List<string> list = new List<string>();
                                for (var i = 2; i < events.Length; i++)
                                {
                                    list.Add(events[i]);
                                }
                                String[] teams = list.ToArray();
                                teamUserList.Clear();

                                foreach (var item in teams)
                                {
                                    UserTab agent = new UserTab();
                                    string[] agentinfo = item.Split(',');
                                    agent.AgentName = agentinfo[0].Split(':')[1] + " " + agentinfo[1].Split(':')[1];

                                    string[] extension = agentinfo[2].Split(':');

                                    if (extension.Length > 1)
                                    {
                                        agent.Extension = extension[1];
                                    }
                                    agent.LoginId = agentinfo[3].Split(':')[1];
                                    string agentState = agentinfo[4].Split(':')[1];
                                    agent.CallStatus = agentState;
                                    agent.CallStatusColor = "#1BDA6D";
                                    if (agentState != "LOGOUT")
                                    {
                                        string[] t = agentinfo[7].Split(new[] { ':' }, 2);

                                        string[] time = t[1].Split();
                                        //DateTime d = new DateTime((int)time[5], time[1], time[2]);
                                        DateTime date = DateTime.Parse(time[3], System.Globalization.CultureInfo.CurrentCulture);

                                        DateTime now = DateTime.Now;
                                        int diffInSeconds = (int)(now - date).TotalSeconds;
                                        agent.Time = diffInSeconds;
                                    }
                                    teamUserList.Add(agent);
                                }
                                teamUserList = teamUserList.Where(x => x.CallStatus != "LOGOUT").ToList();
                                if (teamUserList.Count > 1)
                                {
                                    StackNoData.Visibility = Visibility.Collapsed;
                                    dgSimple.ItemsSource = teamUserList;
                                    dgSimple.Items.Refresh();
                                }
                                else
                                {

                                    dgSimple.ItemsSource = teamUserList;
                                    dgSimple.Items.Refresh();
                                    StackNoData.Visibility = Visibility.Visible;
                                    dgSimple.Height = Double.NaN;
                                }
                                if (!timerOnce)
                                {

                                    this.StartMockValue();
                                    this.StartRefreshDataGrid();
                                    timerOnce = true;
                                }
                                break;
                        }
                    });
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public T GetEnumValue<T>(string value)
        {
            T selectedValue = (T)Enum.Parse(typeof(T), "NONE");
            try
            {
                selectedValue = (T)Enum.Parse(typeof(T), value, true);
            }
            catch (Exception ex)
            {

            }
            return selectedValue;
        }

        private void SetPhoneBookSource(List<UserTab> phoneBookSource)
        {
            PhoneBookStackPanel.Children.Clear();
            foreach (var item in phoneBookSource)
            {
                StackPanel mainPhoneBookStackPanel = new StackPanel();
                Expander expender = DynamicControlUtility.GetExpenderPhoneBook(item.AgentName);
                Border b = DynamicControlUtility.GetBorderPhoneBook("#C8C6C6", 5, 5, 5, 1);

                StackPanel phoneNoStackPanel = new StackPanel();
                TextBlock tb = DynamicControlUtility.GetTextBlockPhoneBook("Phone No");

                TextBox txt = DynamicControlUtility.GetTextBoxPhoneBook(item.Extension);
                phoneNoStackPanel.Children.Add(tb);
                phoneNoStackPanel.Children.Add(txt);
                b.Child = phoneNoStackPanel;


                //notes penal
                Border b1 = DynamicControlUtility.GetBorderPhoneBook("#C8C6C6", 5, 5, 5, 1);

                StackPanel notesStackPanel = new StackPanel();
                TextBlock tb1 = DynamicControlUtility.GetTextBlockPhoneBook("Notes");

                TextBox txt1 = DynamicControlUtility.GetTextBoxPhoneBook("Note");
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
                while (true)
                {
                    foreach (var user in this.teamUserList)
                    {
                        if (user.CallStatus != "LOGOUT")
                        {
                            user.Time = user.Time + 1;
                        }
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
                    foreach (var user in this.teamUserList)
                    {
                        if (user.CallStatus != "LOGOUT")
                        {
                            if (user.DisplayTime != user.Time)
                            {
                                user.DisplayTime = user.Time;
                            }
                        }
                        // NOTE: update if the time changed.

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


        private void Btn_ClickPhoneBook(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;

            if (!string.IsNullOrEmpty(txtPhoneBookSearch.Text.ToLower()))
            {
                PhoneBookPagination.Children.Clear();
                int totalItem = 0;

                us = user.GetUsers();
                string searchtext = txtPhoneBookSearch.Text.ToLower();
                if (string.IsNullOrEmpty(searchtext))
                {
                    //dgSimple.ItemsSource = u;
                    totalItem = us.Count();

                }
                else
                {
                    us = us.Where(x => x.AgentName.ToLower().Contains(searchtext) || x.Extension.ToLower().Contains(searchtext)).ToList();
                    totalItem = us.Count();
                    // dgSimple.ItemsSource = u.Where(x => x.AgentName.ToLower().Contains(searchtext));
                }



                int page_ = Convert.ToInt32(bt.Tag);
                Pager pg = new Pager(totalItem, page_);
                int setHeight = totalItem % pg.PageSize;

                PhoneBookScrollViewerSetHeight(setHeight, page_, pg.EndPage);
                List<UserTab> phoneBookSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize).ToList();
                SetPhoneBookSource(phoneBookSource);
                if (pg.EndPage > 1)
                {
                    if (pg.CurrentPage > 1)
                    {
                        Button First = new Button();
                        First.Name = "FirstPage";
                        First.Click += Btn_ClickPhoneBook;
                        First.Content = "<<";
                        First.Tag = 1;
                        First.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(First);

                        Button Previous = new Button();
                        Previous.Click += Btn_ClickPhoneBook;
                        Previous.Content = "<";
                        Previous.Tag = pg.CurrentPage - 1;
                        Previous.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(Previous);
                    }
                    for (var page = pg.StartPage; page <= pg.EndPage; page++)
                    {
                        Button btn = new Button();
                        btn.Click += Btn_ClickPhoneBook;
                        btn.Content = page;
                        btn.Tag = page;
                        btn.Style = (Style)FindResource("btnPagination");
                        if (page == page_)
                        {

                            btn.FontWeight = FontWeights.UltraBold;
                        }
                        PhoneBookPagination.Children.Add(btn);

                    }
                    if (pg.CurrentPage < pg.TotalPages)
                    {
                        Button Next = new Button();
                        Next.Click += Btn_ClickPhoneBook;
                        Next.Content = ">";
                        Next.Tag = pg.CurrentPage + 1;
                        Next.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(Next);

                        Button Last = new Button();
                        Last.Click += Btn_ClickPhoneBook;
                        Last.Content = ">>";
                        Last.Tag = pg.TotalPages;
                        Last.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(Last);
                    }
                }
            }
            else
            {
                //MessageBox.Show(bt.Tag.ToString());
                PhoneBookPagination.Children.Clear();


                us = user.GetUsers();

                int totalItem = us.Count();
                int page_ = Convert.ToInt32(bt.Tag);
                Pager pg = new Pager(totalItem, page_);
                int setHeight = totalItem % pg.PageSize;

                PhoneBookScrollViewerSetHeight(setHeight, page_, pg.EndPage);
                List<UserTab> phoneBookSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize).ToList();
                SetPhoneBookSource(phoneBookSource);
                if (pg.EndPage > 1)
                {
                    if (pg.CurrentPage > 1)
                    {
                        Button First = new Button();
                        First.Name = "FirstPage";
                        First.Click += Btn_ClickPhoneBook;
                        First.Content = "<<";
                        First.Tag = 1;
                        First.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(First);

                        Button Previous = new Button();
                        Previous.Click += Btn_ClickPhoneBook;
                        Previous.Content = "<";
                        Previous.Tag = pg.CurrentPage - 1;
                        Previous.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(Previous);
                    }
                    for (var page = pg.StartPage; page <= pg.EndPage; page++)
                    {
                        Button btn = new Button();
                        btn.Click += Btn_ClickPhoneBook;
                        btn.Content = page;
                        btn.Tag = page;
                        btn.Style = (Style)FindResource("btnPagination");
                        if (page == page_)
                        {

                            btn.FontWeight = FontWeights.UltraBold;
                        }
                        PhoneBookPagination.Children.Add(btn);

                    }
                    if (pg.CurrentPage < pg.TotalPages)
                    {
                        Button Next = new Button();
                        Next.Click += Btn_ClickPhoneBook;
                        Next.Content = ">";
                        Next.Tag = pg.CurrentPage + 1;
                        Next.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(Next);

                        Button Last = new Button();
                        Last.Click += Btn_ClickPhoneBook;
                        Last.Content = ">>";
                        Last.Tag = pg.TotalPages;
                        Last.Style = (Style)FindResource("btnPagination");
                        PhoneBookPagination.Children.Add(Last);
                    }
                }

            }
        }


        private void PhoneBookScrollViewerSetHeight(int setHeight, int page_, int endPage)
        {
            if (setHeight != 0 && page_ == endPage)
            {
                PhoneBookScrollViewer.Height = Double.NaN;
            }
            else
            {
                PhoneBookScrollViewer.Height = 400;
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



        private void PhoneBookSearch_KeyDown(object sender, KeyEventArgs e)
        {
            PhoneBookPagination.Children.Clear();
            var tct = (TextBox)sender;
            int totalItem = 0;
            us = user.GetUsers();
            string searchtext = txtPhoneBookSearch.Text.ToLower();
            string searchtext1 = txtPhoneBookSearch1.Text.ToLower();

            if (string.IsNullOrEmpty(searchtext))
            {
                //dgSimple.ItemsSource = u;
                totalItem = us.Count();
                PhoneBookNoDataStack.Visibility = Visibility.Collapsed;
            }
            else
            {
                us = us.Where(x => x.AgentName.ToLower().Contains(searchtext) || x.Extension.ToLower().Contains(searchtext)).ToList();
                totalItem = us.Count();
                if (totalItem == 0)
                {
                    PhoneBookNoDataStack.Visibility = Visibility.Visible;
                }
                else
                {
                    PhoneBookNoDataStack.Visibility = Visibility.Collapsed;
                    //dgSimple.Visibility = Visibility.Visible;
                }
                // dgSimple.ItemsSource = u.Where(x => x.AgentName.ToLower().Contains(searchtext));
            }


            if (totalItem <= 10)
            {
                PhoneBookScrollViewer.Height = Double.NaN;
            }
            else
            {
                PhoneBookScrollViewer.Height = 400;
            }

            Pager pg = new Pager(totalItem, 1);
            List<UserTab> phoneBookSource = us.Skip((pg.CurrentPage - 1) * pg.PageSize).Take(pg.PageSize).ToList();
            SetPhoneBookSource(phoneBookSource);

            if (pg.EndPage > 1)
            {
                if (pg.CurrentPage > 1)
                {
                    Button First = new Button();
                    First.Name = "FirstPage";
                    First.Click += Btn_ClickPhoneBook;
                    First.Content = "<<";
                    First.Tag = 1;
                    First.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(First);

                    Button Previous = new Button();
                    Previous.Click += Btn_ClickPhoneBook;
                    Previous.Content = "<";
                    Previous.Tag = pg.CurrentPage - 1;
                    Previous.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(Previous);
                }
                for (var page = pg.StartPage; page <= pg.EndPage; page++)
                {
                    Button btn = new Button();
                    btn.Click += Btn_ClickPhoneBook;
                    btn.Content = page;
                    btn.Tag = page;
                    btn.Style = (Style)FindResource("btnPagination");
                    if (page == 1)
                    {

                        btn.FontWeight = FontWeights.UltraBold;
                    }
                    PhoneBookPagination.Children.Add(btn);

                }
                if (pg.CurrentPage < pg.TotalPages)
                {
                    Button Next = new Button();
                    Next.Click += Btn_ClickPhoneBook;
                    Next.Content = ">";
                    Next.Tag = pg.CurrentPage + 1;
                    Next.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(Next);

                    Button Last = new Button();
                    Last.Click += Btn_ClickPhoneBook;
                    Last.Content = ">>";
                    Last.Tag = pg.TotalPages;
                    Last.Style = (Style)FindResource("btnPagination");
                    PhoneBookPagination.Children.Add(Last);
                }
            }
        }




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

        private void CTITeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var comboBox = (ComboBox)sender;
            if (comboBox.IsLoaded)
            {
                var cb = (ComboBoxPair)comboBox.SelectedItem;
                string _key = cb._Key;
                string _value = cb._Value;
                aMQManager.SendMessageToQueue(GCMessages("getteamusers"), _key + ",true");
            }
        }

        //private void dgSimple_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (dgSimple.IsLoaded)
        //    {
        //        UserTab selectedRow = (UserTab)dgSimple.SelectedItem;

        //        aMQManager.SendMessageToQueue(GCMessages("MakeReady", selectedRow.LoginId), "");
        //        MessageBox.Show("hello");
        //    }
        //}

        private void TeamPerformanceMakeReady_Click(object sender, RoutedEventArgs e)
        {
            UserTab selectedRow = (UserTab)dgSimple.SelectedItem;

            aMQManager.SendMessageToQueue(GCMessages("MakeReady", selectedRow.LoginId), "");
        }

        private void TeamPerformanceNotReady_Click(object sender, RoutedEventArgs e)
        {
            UserTab selectedRow = (UserTab)dgSimple.SelectedItem;
            aMQManager.SendMessageToQueue(GCMessages("MakeNotReady", selectedRow.LoginId), "");
        }

        private void TeamPerformanceSignOut_Click(object sender, RoutedEventArgs e)
        {
            UserTab selectedRow = (UserTab)dgSimple.SelectedItem;
            aMQManager.SendMessageToQueue(GCMessages("logoutwithreason", selectedRow.LoginId), "28");
        }

        private void TeamPerformanceMonitoring_Click(object sender, RoutedEventArgs e)
        {

            UserTab selectedRow = (UserTab)dgSimple.SelectedItem;
            if (selectedRow != null)
            {
                aMQManager.SendMessageToQueue(GCMessages("silentmonitor"), selectedRow.Extension);
            }
            else
            {
                MessageBox.Show("please select row");
            }

        }
    }

    public class UserTab : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string LoginId { get; set; }
        public string AgentName { get; set; }

        public string extension { get; set; }
        public string Extension
        {
            get { return extension; }
            set
            {
                extension = value;
                this.OnPropertyChange(nameof(this.Extension));
            }
        }
        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string RowDetail { get; set; }

        public int ActiveParticipants { get; set; }
        public int HeldParticipants { get; set; }
        public int Duration { get; set; }
        private string callStatus;
        public string CallStatus
        {
            get { return callStatus; }
            set
            {
                callStatus = value;
                this.OnPropertyChange(nameof(this.CallStatus));
            }
        }


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


        public string DisplayTimeString => CallStatus != "LOGOUT" ? $"{ (displayTime / 60 / 60).ToString().PadLeft(2, '0')}:{(displayTime / 60 % 60).ToString().PadLeft(2, '0')}:{(displayTime % 60).ToString().PadLeft(2, '0')}" : "--";


        public List<UserTab> GetUsers()
        {
            List<UserTab> users = new List<UserTab>();
            users.Add(new UserTab() { CallStatusColor = "#1BDA6D", Time = 100, QueueName = "AMQ", isRowDetail = false, CallStatus = "Ready", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "ZZJohn Doe", Extension = "42033", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#d70d0d", Time = 200, QueueName = "AMQ", isRowDetail = false, CallStatus = "Not Ready - Lunch", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 3, AgentName = "John Doe", Extension = "42034", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 210, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 1, AgentName = "ZZJohn Doe", Extension = "42035", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new UserTab() { CallStatusColor = "#E6A30C", Time = 220, QueueName = "AMQ", isRowDetail = true, CallStatus = "Talking", Duration = 1000, HeldParticipants = 500, ActiveParticipants = 30, Id = 2, AgentName = "ZZAJohn Doe", Extension = "42036", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
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


    public enum EventType
    {
        NewInboundCall,
        InboundCall,
        ConsultCall,
        State,
        Error,
        DialogStatus,
        SYSTEM,
        ReasonCodes,
        AgentInfo,
        RECONNECT,
        NOT_RECHEABLE,
        NONE,
        Resumed,
        Interrupted,
        DESTINATION,
        DialogState,
        OUT_OF_SERVICE,
        IN_SERVICE,
        Control,
        NewOutboundCall,
        TeamUsersList
    }

    public enum AgentState
    {
        TALKING,
        HOLD,
        READY,
        NOT_READY,
        WORK,
        WORK_READY,
        RESERVED,
        LOGOUT,
        RE_LOGIN,
        NONE
    }

}