using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        UserTab selectedRow = null;
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
                                                        item.Time = 1;
                                                        item.Extension = agentExtension;
                                                    }
                                                }
                                                if (selectedRow == null)
                                                {
                                                    TeamPerformanceStackPanel.Visibility = Visibility.Collapsed;
                                                }
                                                else if (selectedRow.LoginId == events[8])
                                                {
                                                    dgSimple.SelectedItem = CheckAlreadyExist;
                                                    UpdateTeamPerformanceButtonPanel_NOTREADY();
                                                }
                                                else
                                                {

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
                                                StackNoData.Visibility = Visibility.Collapsed;
                                                dgSimple.ItemsSource = teamUserList;
                                                dgSimple.Items.Refresh();
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
                                            UserTab setItem = new UserTab();
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 1;
                                                    item.Extension = agentExtension;
                                                    setItem = item;
                                                }
                                            }
                                            if (selectedRow == null)
                                            {
                                                TeamPerformanceStackPanel.Visibility = Visibility.Collapsed;
                                            }
                                            else if (selectedRow.LoginId == events[8])
                                            {
                                                dgSimple.SelectedItem = setItem;
                                                UpdateTeamPerformanceButtonPanel_READY();
                                            }
                                            else
                                            {

                                            }
                                            //dgSimple.Items.Refresh();
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }

                                        break;
                                    case AgentState.RESERVED:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //CallEventInfoListing.isLoggedIn = true;
                                            //checkforNotReadyItemsNWrapupReasonLabels();
                                            //CallInfoData callReservedInfoData = new CallInfoData();
                                            //callReservedInfoData.CurrentCallState = Enum.GetName(typeof(CallClassProvider.CallState), CallClassProvider.CallState.Accepted);
                                            //CallStateChangeEvent(this, new CtiCoreEventArgs("CallStateChanged", callReservedInfoData, events[2]));
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 1;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                        }

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
                                            btnTeamPerformanceMonitoring.IsEnabled = true;

                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 1;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }

                                        break;
                                    case AgentState.WORK_READY:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //AgentStateWrapUp(events[2]);
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 1;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }



                                        break;
                                    case AgentState.WORK:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //AgentStateWrapUp(events[2]);
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 1;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }


                                        break;
                                    case AgentState.LOGOUT:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //DeactivateConnection();
                                        }
                                        else
                                        {
                                            try
                                            {
                                                foreach (var item in teamUserList)
                                                {
                                                    if (item.LoginId == events[8])
                                                    {
                                                        item.CallStatus = eAgentState.ToString();
                                                        item.Extension = "";
                                                        item.Time = 1;
                                                    }
                                                }
                                                teamUserList = teamUserList.Where(x => x.CallStatus != "LOGOUT").ToList();
                                                //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                                if (teamUserList.Count > 0)
                                                {
                                                    StackNoData.Visibility = Visibility.Collapsed;
                                                    dgSimple.ItemsSource = teamUserList;
                                                    dgSimple.Items.Refresh();
                                                }
                                                else
                                                {
                                                    dgSimple.Height = Double.NaN;
                                                    dgSimple.ItemsSource = teamUserList;
                                                    dgSimple.Items.Refresh();
                                                    Dispatcher.Invoke(() => StackNoData.Visibility = Visibility.Visible);
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                                throw ex;
                                            }


                                        }

                                        break;
                                    case AgentState.RE_LOGIN:
                                        //DeactivateConnection();
                                        break;
                                    case AgentState.HOLD:
                                        if (CallEventInfoListing.agentID == events[8])
                                        {
                                            //CallStateHold(CallEventInfoListing.currentActiveCall);
                                        }
                                        else
                                        {
                                            foreach (var item in teamUserList)
                                            {
                                                if (item.LoginId == events[8])
                                                {
                                                    item.CallStatus = eAgentState.ToString();
                                                    item.Time = 1;
                                                    item.Extension = agentExtension;
                                                }
                                            }
                                            //teamUserList.Where(x => x.LoginId == events[8]).FirstOrDefault().CallStatus = eAgentState.ToString();
                                        }

                                        break;
                                }
                                break;
                            case EventType.AgentInfo:
                                CallEventInfoListing.isWraupAllowed = !events[5].ToUpperInvariant().Equals("NOT_ALLOWED");


                                if (Convert.ToBoolean(events[4]) == true)
                                {
                                    CallEventInfoListing.agentID = events[0];
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
                                        try
                                        {

                                            string[] t = agentinfo[7].Split(new[] { ':' }, 2);

                                            //string[] time = t[1].Split();
                                            //DateTime d = new DateTime((int)time[5], time[1], time[2]);
                                            DateTime date = DateTime.Parse(t[1], System.Globalization.CultureInfo.CurrentCulture);

                                            DateTime now = DateTime.Now;
                                            int diffInSeconds = (int)(now - date).TotalSeconds;
                                            agent.Time = diffInSeconds;
                                        }
                                        catch (Exception ex)
                                        {

                                            throw ex;
                                        }
                                    }
                                    teamUserList.Add(agent);
                                }
                               // teamUserList = teamUserList.Where(x => x.CallStatus != "LOGOUT").ToList();
                                if (teamUserList.Count > 0)
                                {
                                    StackNoData.Visibility = Visibility.Collapsed;
                                    dgSimple.ItemsSource = teamUserList;
                                    dgSimple.Items.Refresh();
                                    if (teamUserList.Count >= 10)
                                    {
                                        dgSimple.Height = 350;
                                    }
                                    else
                                    {
                                        dgSimple.Height = Double.NaN;
                                    }
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

                            case EventType.NewInboundCall:
                                string dialogID = events[4].Split(':')[1];

                                CallEventInfo objCallEventInfo1 = CallEventInfoListing.lstCallEventInfo.Where(r => r.MyCallInfoData.CallId == dialogID).FirstOrDefault();
                                CallEventInfoListing.isOutBoundCall = false;
                                CallEventInfoListing.callAction = string.Empty;
                                CallEventInfoListing.participants = null;


                                // only process NewInboundcall when no call with same Dialog ID exists in the system
                                if (objCallEventInfo1 == null)
                                {
                                    CallEventInfoListing.activityDialogID = dialogID;
                                    CallInfoData callInfoData = new CallInfoData();
                                    callInfoData.Ani = events[2];
                                    callInfoData.CallId = dialogID;
                                    callInfoData.Dnis = events[2];
                                    callInfoData.CallReceived = DateTime.Now;

                                    //callInfoData.CurrentCallState = Enum.GetName(typeof(CallClassProvider.CallState), CallClassProvider.CallState.Ringing);

                                    callInfoData.CallType = CallType.InboundCall.ToString();

                                    objCallEventInfo1 = new CallEventInfo();
                                    objCallEventInfo1.Duration = 0;
                                    //objCallEventInfo.AgentID = _AgentRefId;
                                    objCallEventInfo1.MyCallInfoData = callInfoData;
                                    objCallEventInfo1.callVariables = events[3].Split('|').ToList();
                                    CallEventInfoListing.lstCallEventInfo.Add(objCallEventInfo1);

                                    CallEventInfoListing.previousDialogID = CallEventInfoListing.getValueOfGivenVariable(objCallEventInfo1.callVariables, "associatedDialogId");
                                    //call back number
                                    if (string.IsNullOrEmpty(CallEventInfoListing.previousDialogID))
                                    {
                                        CallEventInfoListing.callBackNumber = events[2];
                                    }
                                    //----------
                                    CallEventInfoListing.currentActiveCall = dialogID;
                                }
                                else
                                {
                                    objCallEventInfo1.callVariables = events[3].Split('|').ToList();
                                    objCallEventInfo1.MyCallInfoData.CallType = CallType.InboundCall.ToString();
                                    if (string.IsNullOrEmpty(CallEventInfoListing.previousDialogID))
                                    {
                                        CallEventInfoListing.callBackNumber = events[2];
                                    }
                                }

                                break;


                            case EventType.InboundCall:
                                string callstate = string.Empty;
                                string dialogIDInboundCall = string.Empty;

                                if (events.Length >= 4)
                                {
                                    string[] dialogInboundCall = events[3].Split(':').ToArray();

                                    if (dialogInboundCall.Length > 1)
                                    {
                                        dialogIDInboundCall = dialogInboundCall[1];
                                    }
                                }
                                VendorCallState callStateEvent = GetEnumValue<VendorCallState>(events[2]);
                                #region inbound call states

                                switch (callStateEvent)
                                {
                                    case VendorCallState.INITIATED:
                                        //MakeOutboundCall(events, dialogIDInboundCall, CallType.OutboundCall.ToString());
                                        break;
                                    case VendorCallState.INITIATING:
                                        // MakeOutboundCall(events, dialogIDInboundCall, CallType.OutboundCall.ToString());
                                        break;
                                    case VendorCallState.ACTIVE:
                                        string callType = events[7].Split(':')[1];
                                        if (callType == CallType.SM.ToString() && !string.IsNullOrEmpty(dialogIDInboundCall))
                                        {
                                            try
                                            {
                                                CallEventInfoListing.isOutBoundCall = false;
                                                CallEventInfoListing.callAction = string.Empty;
                                                CallEventInfoListing.participants = null;




                                                CallInfoData callMonitorInfoData = new CallInfoData();
                                                callMonitorInfoData.CallId = dialogIDInboundCall;
                                                callMonitorInfoData.Ani = events[4];
                                                callMonitorInfoData.Dnis = events[4];
                                                List<string> callVariable = events[5].Split('|').ToList();
                                                callMonitorInfoData.CallReceived = DateTime.Now;
                                                callMonitorInfoData.CallType = callType;

                                                txtMonitoringCallingNumber.Text = events[4];


                                                CallEventInfo monitorcall = new CallEventInfo();
                                                monitorcall.MyCallInfoData = callMonitorInfoData;
                                                monitorcall.callVariables = events[5].Split('|').ToList();
                                                CallEventInfoListing.lstCallEventInfo.Clear();
                                                ; CallEventInfoListing.lstCallEventInfo.Add(monitorcall);
                                                btnTeamPerformanceMonitoring.Visibility = Visibility.Collapsed;
                                                btnTeamPerformanceEndMonitoring.Visibility = Visibility.Visible;
                                                //monitorcall.MyCallInfoData.CurrentCallState = "Connected";

                                            }
                                            catch (Exception ex)
                                            {

                                                throw ex;
                                            }
                                        }
                                        else if (callType == CallType.InboundCall.ToString() && !string.IsNullOrEmpty(dialogIDInboundCall))
                                        {
                                            CallEventInfo call = CallEventInfoListing.lstCallEventInfo.Where(obj => obj.MyCallInfoData.CallId == dialogIDInboundCall).FirstOrDefault();
                                            //callstate = Enum.GetName(typeof(CallClassProvider.CallState), CallClassProvider.CallState.Connected);
                                            if (call != null)
                                            {
                                                // this is normal call, which already exist in system in ringing state
                                                //log.Debug("EventType.InboundCall InboundCall#Active normal call, which already exist in system");
                                                call.MyCallInfoData.CurrentCallState = "Connected";
                                                call.callVariables = events[5].Split('|').ToList();

                                                // capture the conference events
                                                if (CallEventInfoListing.lstCallEventInfo.Count == 1 && events.Length >= 10)
                                                {
                                                    string[] participatnsinConfCall = events[9].Split(':').ToArray();

                                                    if (participatnsinConfCall.Length > 1)
                                                    {

                                                        CallEventInfoListing.participants = null;
                                                        CallEventInfoListing.participants = new List<string>();

                                                        CallEventInfoListing.participants = participatnsinConfCall[1].Split('|').ToList();
                                                    }

                                                }


                                                if (CallEventInfoListing.lstCallEventInfo.Count == 1 &&
                                                    (call.MyCallInfoData.CallType == CallType.ConsultCall.ToString() || string.IsNullOrEmpty(call.MyCallInfoData.CallType)))
                                                {
                                                    call.MyCallInfoData.CallType = CallType.InboundCall.ToString();
                                                }

                                                if (isCallStartTimeEmpty(events, call.istimeDifferenceCalculated))
                                                {
                                                    call.callStartTime = events[6];
                                                    call.timeDifference = CallEventInfoListing.timeDifference(call.callStartTime);
                                                    call.istimeDifferenceCalculated = true;
                                                }
                                                else
                                                {
                                                    call.callStartTime = string.Empty;
                                                }

                                                if (call.callstopwatch == null)
                                                {
                                                    call.callstopwatch = new Stopwatch();
                                                }
                                                if (!call.callstopwatch.IsRunning)
                                                {
                                                    call.callstopwatch.Start();
                                                }
                                                //------------------------------------------------------------------
                                                CallEventInfoListing.currentActiveCall = dialogIDInboundCall;
                                                // CallStateChangeEvent(this, new CtiCoreEventArgs("CallStateChanged", call.MyCallInfoData, events[2]));
                                            }
                                            else // no call exist with this dialog
                                            {
                                                //Neither consult nor inbound call exist with the current DialogID
                                                // there can be 3 cases:
                                                //1 conference call
                                                //2 consultative transfer
                                                //3 customer has dropped the call while consult was in progress.
                                                //log.Debug("EventType.InboundCall InboundCall#Active call doesnot already exist in system");
                                                CallInfoData callInfoData = new CallInfoData();
                                                callInfoData.Ani = events[4];
                                                callInfoData.CallId = dialogIDInboundCall;
                                                CallEventInfoListing.activityDialogID = dialogIDInboundCall;
                                                callInfoData.Dnis = events[4];
                                                callInfoData.CallReceived = DateTime.Now;
                                                callInfoData.CurrentCallState = callstate;
                                                // callInfoData.CallType = CallType.InboundCall.ToString();
                                                callInfoData.CallType = CallType.OutboundCall.ToString(); // change for the 
                                                CallEventInfo objCallEventInfo = new CallEventInfo();
                                                objCallEventInfo.Duration = 0;
                                                //uzair
                                                //objCallEventInfo.AgentID = _AgentRefId;

                                                objCallEventInfo.MyCallInfoData = callInfoData;
                                                objCallEventInfo.callVariables = events[5].Split('|').ToList();
                                                //------------date time -----------------------------------
                                                if (isCallStartTimeEmpty(events, objCallEventInfo.istimeDifferenceCalculated))
                                                {
                                                    objCallEventInfo.callStartTime = events[6];
                                                    objCallEventInfo.timeDifference = CallEventInfoListing.timeDifference(objCallEventInfo.callStartTime);
                                                    objCallEventInfo.istimeDifferenceCalculated = true;
                                                }
                                                else
                                                {
                                                    objCallEventInfo.callStartTime = string.Empty;
                                                }

                                                if (objCallEventInfo.callstopwatch == null)
                                                {
                                                    objCallEventInfo.callstopwatch = new Stopwatch();
                                                }
                                                if (!objCallEventInfo.callstopwatch.IsRunning)
                                                {
                                                    objCallEventInfo.callstopwatch.Start();
                                                }
                                                //--------------------------------------------------------------
                                                CallEventInfoListing.lstCallEventInfo.Add(objCallEventInfo);
                                                CallEventInfoListing.currentActiveCall = callInfoData.CallId;
                                                // CallStateChangeEvent(this, new CtiCoreEventArgs("CallStateChanged", callInfoData, events[4]));
                                                // In order to resolve the issue that customer drops the call while consult was in ringing / held state. GetDialogState is sent.
                                                CtiCommands.GetCurrentDialogState();
                                            }
                                        }



                                        break;
                                    case VendorCallState.FAILED:

                                        string error = string.IsNullOrEmpty(events[4]) ? "Call Failed" : events[4];
                                        //SystemStateUnKnown(error);

                                        //if call fails, user will be unable to do anything.
                                        //therefore, we add a call, so customer can end the call
                                        //otherwise, call will be dropped by itself after around 15 sec (as defined on CUCM)
                                        //MakeOutboundCall(events, events[3].Split(':')?[1], CallType.InboundCall.ToString());
                                        break;
                                    case VendorCallState.DROP:
                                    case VendorCallState.DROPPED:
                                        //CallStateDrop(dialogIDInboundCall);
                                        btnTeamPerformanceMonitoring.Visibility = Visibility.Visible;
                                        btnTeamPerformanceEndMonitoring.Visibility = Visibility.Collapsed;
                                        break;

                                    case VendorCallState.HELD:
                                        CallEventInfo heldcall = CallEventInfoListing.lstCallEventInfo.Where(obj => obj.MyCallInfoData.CallId == dialogIDInboundCall).FirstOrDefault();
                                        if (heldcall != null)
                                        {
                                            // this is normal call, which already exist in system in ringing state
                                            //  log.Debug("EventType.InboundCall InboundCall#HELD updateCallVariables");
                                            heldcall.callVariables = events[5].Split('|').ToList();

                                            //-----------------------------
                                            string callVariable4 = CallEventInfoListing.getValueOfGivenVariable(heldcall.callVariables, "callVariable4");
                                            string callVariable5 = CallEventInfoListing.getValueOfGivenVariable(heldcall.callVariables, "callVariable5");

                                            //log.Info("CTICONNECTOR callVariable4=" + callVariable4 + " ,Context.SessionID=callVariable5= " + callVariable5 + ",CallEventInfoListing.callActionRequested:" + CallEventInfoListing.callActionRequested);


                                            if (CallEventInfoListing.lstCallEventInfo.Count == 2 && CallEventInfoListing.callActionRequested.Equals("TRANSFER") && callVariable4.Equals("TRANSFER"))
                                            {
                                                CallEventInfoListing.callActionRequested = string.Empty;
                                                string transfercmd = string.Concat(heldcall.MyCallInfoData.Ani + ",", heldcall.MyCallInfoData.CallId);
                                                //  log.Debug("transfercmdCTI: " + transfercmd);
                                                //no  wrapup after call transfer so no nned to save the dialog ID 
                                                //CallEventInfoListing.transferCallDialogID = InboundCall.MyCallInfoData.CallId;
                                                CallEventInfoListing.isTransferCall = true;
                                                CallEventInfoListing.callAction = "TRANSFER";
                                                //AMQManager.GetInstance().SendMessage(GCMessages("TransferCall"), transfercmd);

                                                //var CurrentCall = CallEventInfoListing.lstCallEventInfo.Where(obj => obj.MyCallInfoData.CallId == CallEventInfoListing.currentActiveCall).FirstOrDefault();
                                                //if (CurrentCall != null && CurrentCall.MyCallInfoData != null)
                                                //{
                                                //    log.Debug("consultingAgentExtension :" + CurrentCall.MyCallInfoData.Ani);
                                                //    CallEventInfoListing.consultingAgentExtension = CurrentCall.MyCallInfoData.Ani;
                                                //}
                                                //else
                                                //{
                                                //    log.Debug("CurrentCall: " + transfercmd);
                                                //}
                                            }
                                            else if (CallEventInfoListing.lstCallEventInfo.Count == 2 && CallEventInfoListing.callActionRequested.Equals("CONFERENCE") && callVariable4.Equals("CONFERENCE"))
                                            {
                                                CallEventInfoListing.callActionRequested = string.Empty;
                                                var ConferenceCall = CallEventInfoListing.lstCallEventInfo.Where(obj => obj.MyCallInfoData.CallId != CallEventInfoListing.currentActiveCall).FirstOrDefault();
                                                //var ConferenceCall = CallEventInfoListing.lstCallEventInfo.Where(obj => obj.MyCallInfoData.CallId == CallEventInfoListing.activityDialogID ).FirstOrDefault();
                                                //log.Debug("ConferenceCall.MyCallInfoData?.CallIdctiConnector :" + ConferenceCall?.MyCallInfoData?.CallId + "callActionRequested:" + CallEventInfoListing.callActionRequested + " currentActiveCall" + CallEventInfoListing.currentActiveCall);
                                                if (ConferenceCall != null && ConferenceCall.MyCallInfoData != null)
                                                {
                                                    //var CurrentCall = CallEventInfoListing.lstCallEventInfo.Where(obj => obj.MyCallInfoData.CallId == CallEventInfoListing.currentActiveCall).FirstOrDefault();
                                                    //if (CurrentCall != null && CurrentCall.MyCallInfoData != null)
                                                    //{
                                                    //    log.Debug("Current call Ani :" + CurrentCall.MyCallInfoData.Ani + ",consultingAgentExtension="+ CallEventInfoListing.consultingAgentExtension);
                                                    //   // CallEventInfoListing.consultingAgentExtension = CurrentCall.MyCallInfoData.Ani;
                                                    //}

                                                    //CallEventInfoListing.callActionRequested = "CONFERENCE";
                                                    CallEventInfoListing.isConference = true;
                                                    //  string conferencecmd = string.Concat(tbDial1.Text + ",", ConferenceCall.MyCallInfoData.CallId);
                                                    string conferencecmd = string.Concat(CallEventInfoListing.consultingAgentExtension + ",", ConferenceCall.MyCallInfoData.CallId);
                                                    //  log.Debug("conferencecmd :" + conferencecmd);
                                                    CallEventInfoListing.callAction = "CONFERENCE";
                                                    AMQManager.GetInstance().SendMessageToQueue(GCMessages("ConferenceCall"), conferencecmd);

                                                }
                                                else
                                                {
                                                    int callno = 1;
                                                    foreach (var item in CallEventInfoListing.lstCallEventInfo)
                                                    {
                                                        if (item != null && item.MyCallInfoData != null)
                                                        {
                                                        }
                                                        else
                                                        {
                                                        }
                                                        callno++;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                }
                                #endregion inboundcall states
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

        private void dgSimple_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSimple.IsLoaded)
            {
                selectedRow = (UserTab)dgSimple.SelectedItem;

                if (selectedRow != null)
                {
                    TeamPerformanceStackPanel.Visibility = Visibility.Visible;
                    if (selectedRow.CallStatus == "NOT_READY")
                    {
                        UpdateTeamPerformanceButtonPanel_NOTREADY();
                    }
                    else if (selectedRow.CallStatus == "READY")
                    {
                        UpdateTeamPerformanceButtonPanel_READY();

                    }
                    else if (selectedRow.CallStatus == "LOGOUT")
                    {
                        UpdateTeamPerformanceButtonPanel_LOGOUT();
                    }
                    else if (selectedRow.CallStatus == "TALKING")
                    {
                        UpdateTeamPerformanceButtonPanel_TALKING();
                    }
                }
            }
        }

        private void UpdateTeamPerformanceButtonPanel_NOTREADY()
        {
            btnTeamPerformanceMonitoring.IsEnabled = false;
            btnTeamPerformanceEndMonitoring.IsEnabled = false;
            btnTeamPerformanceNotReady.IsEnabled = false;
            btnTeamPerformanceMakeReady.IsEnabled = true;
            btnTeamPerformanceSignOut.IsEnabled = true;
            btnTeamPerformanceHold.IsEnabled = false;
            btnTeamPerformanceHeld.IsEnabled = false;
        }

        private void UpdateTeamPerformanceButtonPanel_READY()
        {
            btnTeamPerformanceMonitoring.IsEnabled = false;
            btnTeamPerformanceEndMonitoring.IsEnabled = false;
            btnTeamPerformanceNotReady.IsEnabled = true;
            btnTeamPerformanceMakeReady.IsEnabled = false;
            btnTeamPerformanceSignOut.IsEnabled = true;
            btnTeamPerformanceHold.IsEnabled = false;
            btnTeamPerformanceHeld.IsEnabled = false;
        }
        private void UpdateTeamPerformanceButtonPanel_LOGOUT()
        {
            btnTeamPerformanceMonitoring.IsEnabled = false;
            btnTeamPerformanceEndMonitoring.IsEnabled = false;
            btnTeamPerformanceNotReady.IsEnabled = false;
            btnTeamPerformanceMakeReady.IsEnabled = false;
            btnTeamPerformanceSignOut.IsEnabled = false;
            btnTeamPerformanceHold.IsEnabled = false;
            btnTeamPerformanceHeld.IsEnabled = false;
        }

        private void UpdateTeamPerformanceButtonPanel_TALKING()
        {
            btnTeamPerformanceMonitoring.IsEnabled = true;
            btnTeamPerformanceEndMonitoring.IsEnabled = false;
            btnTeamPerformanceNotReady.IsEnabled = false;
            btnTeamPerformanceMakeReady.IsEnabled = false;
            btnTeamPerformanceSignOut.IsEnabled = true;
            btnTeamPerformanceHold.IsEnabled = false;
            btnTeamPerformanceHeld.IsEnabled = false;
        }

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
            aMQManager.SendMessageToQueue(GCMessages("logout", selectedRow.LoginId), "");
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

        private bool isCallStartTimeEmpty(string[] events, bool istimeDifferenceCalculated)
        {//7th token of call is start datetime from finesse.Agreeed format is '2018-01-10 14:11:44'
            if (events.Length > 6 && !string.IsNullOrEmpty(events[6]) && istimeDifferenceCalculated == false)
            {
                string[] datetimetoken = events[6].Split(' ').ToArray();
                if (datetimetoken.Length > 1)
                {
                    string[] datetoken = datetimetoken[0].Split('-').ToArray();
                    if (datetoken.Length == 3)
                    {
                        string[] timetoken = datetimetoken[1].Split(':').ToArray();
                        if (timetoken.Length == 3)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        private void TeamPerformanceHold_Click(object sender, RoutedEventArgs e)
        {
            string dialogId = CallEventInfoListing.lstCallEventInfo.ElementAt(0).MyCallInfoData.CallId;
            aMQManager.SendMessageToQueue(GCMessages("HoldCall"), dialogId);
        }

        private void TeamPerformanceEndMonitoring_Click(object sender, RoutedEventArgs e)
        {
            string dialogId = CallEventInfoListing.lstCallEventInfo.ElementAt(0).MyCallInfoData.CallId;
            aMQManager.SendMessageToQueue(GCMessages("endsilentmonitor"), dialogId);
        }

        private void TeamPerformanceHeld_Click(object sender, RoutedEventArgs e)
        {
            string dialogId = CallEventInfoListing.lstCallEventInfo.ElementAt(0).MyCallInfoData.CallId;
            aMQManager.SendMessageToQueue(GCMessages("RetrieveCall"), dialogId);
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

    public enum CallType
    {
        ConsultCall,
        InboundCall,
        OutboundCall,
        SM
    }
    public enum VendorCallState
    {
        NONE,
        INITIATING,
        INITIATED,
        ALERTING,
        ACTIVE,
        DROP,
        FAILED,
        HELD,
        NotFound,
        DROPPED
    }


}