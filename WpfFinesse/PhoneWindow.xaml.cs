using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for PhoneWindow.xaml
    /// </summary>
    public partial class PhoneWindow : Window
    {
        AMQManager aMQManager = AMQManager.GetInstance();
        Agent agent = Agent.GetInstance();
        private  int s, m, h = 0;
        Timer time = new Timer(1000);
        public PhoneWindow()
        {
            aMQManager = AMQManager.GetInstance();
            aMQManager.InitializeAMQ();
            aMQManager.messageArrived += AMQManager_messageArrived1;
            aMQManager.UpdateTopic();
            InitializeComponent();
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
                    else if (args.eventArgs[1] == "NewInboundCall")
                    {

                        string[] dialogID = args.eventArgs[4].Split(':');
                       // CallPanel.Visibility = Visibility.Visible;
                        txtCallingNumber.Text = "Calling... " + args.eventArgs[2];
                        CallInfoData.DialogId = dialogID[1];
                    }
                    if (args.eventArgs[1] == "InboundCall")
                    {
                        if (GC_Commands.Inboundcall_current_state.ACTIVE.ToString() == args.eventArgs[2])
                        {
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1);
                            timer.Tick += timer_Tick;
                            timer.Start();
                            s = m = h = 0;
                            //time.Elapsed += Time_Elapsed;
                            //time.Start();

                        }
                        if (GC_Commands.Inboundcall_current_state.DROPPED.ToString() == args.eventArgs[2])
                        {
                            //CallPanel.Visibility = Visibility.Hidden;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1);
                            timer.Start();
                            txtCallTiming.Text = "";
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

        private void btnCallDrop_Click(object sender, RoutedEventArgs e)
        {
            string command = GC_Utility.CreateComand(GC_AllCommand.ReleaseCall.ToString(), agent.AgentID);
            aMQManager.SendMessageToQueue(command, CallInfoData.DialogId);
        }

        private void btnCallPick_Click(object sender, RoutedEventArgs e)
        {
            string command = GC_Utility.CreateComand(GC_AllCommand.AnswerCall.ToString(), agent.AgentID);
            aMQManager.SendMessageToQueue(command, CallInfoData.DialogId);
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

        private void Time_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            Dispatcher.Invoke(() =>
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
            });

        }

        private void btnCallVaribles_Click(object sender, RoutedEventArgs e)
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
