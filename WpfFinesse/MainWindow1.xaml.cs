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
using System.Windows.Threading;
using WpfFinesse.AMQ;
using WpfFinesse.GC_Commands;
using WpfFinesse.Models;
using WpfFinesse.Utility;

namespace WpfFinesse
{
    /// <summary>
    /// Interaction logic for MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        AMQManager aMQManager = AMQManager.GetInstance();
        Agent agent = null;
        private int s, m, h = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool ActiveHoldCall = false;
        public MainWindow1()
        {
            InitializeComponent();
            timer.Tick -= timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            agent = Agent.GetInstance();
            //    string reasonCode = GC_Utility.CreateComand(GC_AllCommand.reasoncodenotready.ToString(), agent.AgentID);
            aMQManager.messageArrived += AMQManager_messageArrived1;
            //    aMQManager.SendMessageToQueue(reasonCode, null);
            //    aMQManager.UpdatatheTopic();
            txtDailNumber.Text = "";


            
        }

        private void AMQManager_messageArrived1(object sender, MyEventArgs args)
        {
            if (args != null)
            {
                Dispatcher.Invoke(() =>
                {
                    if (args.eventArgs[1] == "State")
                    {
                    }
                    else if (args.eventArgs[1] == GC_Events.ReasonCodes.ToString())
                    {
                        AMQManager aMQManager = AMQManager.GetInstance();
                        string[] codes = args.eventArgs[3].Split(',');
                        string[] labels = args.eventArgs[4].Split(',');
                        List<string> list = labels.ToList();
                        list.Add("Not Ready");
                        labels = list.ToArray();
                        if (codes.Length == labels.Length)
                        {
                            aMQManager.dictionary.Clear();
                            for (int i = 0; i < codes.Length; i++)
                            {
                                aMQManager.dictionary.Add(codes[i], labels[i]);
                            }
                        }
                        //foreach (KeyValuePair<string, string> entry in aMQManager.dictionary)
                        //{
                        //    Console.WriteLine(entry.Key + "-" + entry.Value);
                        //}

                        foreach (var item in labels)
                        {
                            var cbitem = new ComboBoxItem();
                            cbitem.Content = item;
                            cbReasonCodes.Items.Add(cbitem);
                        }
                    }
                    else if (args.eventArgs[1] == "NewInboundCall")
                    {

                        string[] dialogID = args.eventArgs[4].Split(':');
                        ConsultCallBar.Visibility = Visibility.Collapsed;

                        CallPanel.Visibility = Visibility.Visible;
                        btnCallPick.Visibility = Visibility.Visible;

                        btnCallEnd.Visibility = Visibility.Collapsed;
                        btnHoldcall.Visibility = Visibility.Collapsed;
                        btnResumecall.Visibility = Visibility.Collapsed;
                        btntransfercall.Visibility = Visibility.Collapsed;
                        btnConsultcall.Visibility = Visibility.Collapsed;
                        txtCallingNumber.Text = args.eventArgs[2];
                        CallInfoData.DialogId = dialogID[1];
                    }
                    else if (args.eventArgs[1] == "InboundCall")
                    {
                        if (GC_Commands.Inboundcall_current_state.ACTIVE.ToString() == args.eventArgs[2])
                        {
                            CallInfoData.CallStatus = args.eventArgs[2];
                            btnCallPick.Visibility = Visibility.Collapsed;
                            btnCallEnd.Visibility = Visibility.Visible;
                            btnHoldcall.Visibility = Visibility.Visible;
                            btntransfercall.Visibility = Visibility.Visible;
                            btnConsultcall.Visibility = Visibility.Visible;
                            if (ActiveHoldCall)
                            {
                                btnResumecall.Visibility = Visibility.Collapsed;
                                btnHoldcall.Visibility = Visibility.Visible;
                                ActiveHoldCall = false;
                            }
                            else
                            {
                                s = m = h = 0;
                                //timer = new DispatcherTimer();
                                timer.Tick -= timer_Tick;
                                timer.Interval = TimeSpan.FromSeconds(1);
                                timer.Tick += timer_Tick;
                                timer.Start();
                            }
                        }
                        else if (GC_Commands.Inboundcall_current_state.DROPPED.ToString() == args.eventArgs[2])
                        {
                            CallPanel.Visibility = Visibility.Hidden;
                            btnCallEnd.Visibility = Visibility.Collapsed;
                            btnHoldcall.Visibility = Visibility.Collapsed;
                            btntransfercall.Visibility = Visibility.Collapsed;
                            btnConsultcall.Visibility = Visibility.Collapsed;
                            txtCallTiming.Text = "";
                            txtDailNumber.Text = "";
                            s = m = h = 0;
                            timer.Stop();
                            timer.Tick -= timer_Tick;


                        }
                        else if (GC_Commands.Inboundcall_current_state.HELD.ToString() == args.eventArgs[2])
                        {
                            btnHoldcall.Visibility = Visibility.Collapsed;
                            btnResumecall.Visibility = Visibility.Visible;
                           // ConsultCallBar.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (args.eventArgs[1] == "ConsultCall")
                    {

                        ConsultCallBar.Visibility = Visibility.Visible;
                        //txtConsultCallingNumber.Text = args.eventArgs[4];
                        btnConsultCallEnd.Visibility = Visibility.Visible;
                        CallInfoData.ConsultDialogId = args.eventArgs[3].Split(':')[1];
                        if (args.eventArgs[2] == "INITIATING")
                        {
                            txtConsultCallingNumber.Text = "INITIATING " + args.eventArgs[4];
                        }
                        else if (args.eventArgs[2] == "INITIATED")
                        {
                            txtConsultCallingNumber.Text = "INITIATED " + args.eventArgs[4];
                        }
                        else if (args.eventArgs[2] == "ALERTING")
                        {
                            txtConsultCallingNumber.Text = "ALERTING " + args.eventArgs[4];
                        }
                        else if (args.eventArgs[2] == "ACTIVE")
                        {
                            txtConsultCallingNumber.Text = "ACTIVE " + args.eventArgs[4];
                        }
                        else if (args.eventArgs[2] == "DROPPED")
                        {
                            //ConsultCallBar.Visibility = Visibility.Collapsed;
                            txtConsultCallingNumber.Text = "DROPPED " + args.eventArgs[4];

                            //txtConsultCallingNumber.Text = "";
                        }
                        else if (args.eventArgs[2] == "FAILED")
                        {
                            //ConsultCallBar.Visibility = Visibility.Collapsed;
                            txtConsultCallingNumber.Text = "FAILED " + args.eventArgs[4];
                            //txtConsultCallingNumber.Text = "";
                        }
                    }

                    else if (args.eventArgs[1] == "OUT_OF_SERVICE")
                    {
                    }
                    else if (args.eventArgs[1] == "IN_SERVICE")
                    {
                    }
                    else if (args.eventArgs[1] == "Error")
                    {

                    }
                });
            }
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

        private void cbReasonCodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox cmb = sender as ComboBox;
            //ComboBoxItem comboBoxItem = new ComboBoxItem();
            //comboBoxItem = (ComboBoxItem)cmb.SelectedItem;
            //if (comboBoxItem.Content.ToString() == "Ready")
            //{
            //    string makeReady = GC_Utility.CreateComand(GC_AllCommand.MakeReady.ToString(), agent.AgentID);
            //    aMQManager.messageArrived += AMQManager_messageArrived1;
            //    aMQManager.SendMessageToQueue(makeReady, null);
            //}
        }

        private void btnCallDrop_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string name = b.Name;
            if (name == "btnCallEnd")
            {
                string command = GC_Utility.CreateComand(GC_AllCommand.ReleaseCall.ToString(), agent.AgentID);
                aMQManager.messageArrived += AMQManager_messageArrived1;
                aMQManager.SendMessageToQueue(command, CallInfoData.DialogId);
            }
            else if (name == "btnConsultCallEnd")
            {
                string command = GC_Utility.CreateComand(GC_AllCommand.ReleaseCall.ToString(), agent.AgentID);
                aMQManager.SendMessageToQueue(command, CallInfoData.ConsultDialogId);
            }
        }

        private void btntransfercall_Click(object sender, RoutedEventArgs e)
        {
            string extension = txtDailNumber.Text;
            string command = GC_Utility.CreateComand(GC_AllCommand.Transfer_sst.ToString(), agent.AgentID);
            aMQManager.SendMessageToQueue(command, extension + "," + CallInfoData.DialogId);

        }

        private void btnHoldcall_Click(object sender, RoutedEventArgs e)
        {
            string command = GC_Utility.CreateComand(GC_AllCommand.HoldCall.ToString(), agent.AgentID);
            aMQManager.SendMessageToQueue(command, CallInfoData.DialogId);
        }

        private void btnResumecall_Click(object sender, RoutedEventArgs e)
        {
            string command = GC_Utility.CreateComand(GC_AllCommand.RetrieveCall.ToString(), agent.AgentID);
            bool result = aMQManager.SendMessageToQueue(command, CallInfoData.DialogId);
            if (result)
            {
                ActiveHoldCall = true;
            }
        }

        private void btnConsultcall_Click(object sender, RoutedEventArgs e)
        {
            string extension = txtDailNumber.Text;
            string command = GC_Utility.CreateComand(GC_AllCommand.ConsultCall.ToString(), agent.AgentID);
            aMQManager.SendMessageToQueue(command, extension + "," + CallInfoData.DialogId);
        }

        private void btnCallPick_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string name = b.Name;
            if (name == "btnCallPick")
            {
                string command = GC_Utility.CreateComand(GC_AllCommand.AnswerCall.ToString(), agent.AgentID);
                aMQManager.SendMessageToQueue(command, CallInfoData.DialogId);
            }
            else if (name == "btnConsultCallPick")
            {

            }

        }

        void timer_Tick(object sender, EventArgs e)
        {
            s++;
            if (s == 60)
            {
                s = 0;
                m++;
            }
            if (m == 60)
            {
                m = 0;
                h++;
            }
            string timerr = String.Format("{0}:{1}:{2}", h.ToString().PadLeft(2, '0'), m.ToString().PadLeft(2, '0'), s.ToString().PadLeft(2, '0'));
            txtCallTiming.Text = timerr;
        }
    }
}
