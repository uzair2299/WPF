using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfFinesse.AMQ;
using WpfFinesse.GC_Commands;
using WpfFinesse.Models;
using WpfFinesse.Utility;

namespace WpfFinesse
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Agent agent = null;
        private string Destination = null;
        public static bool isopen = false;
        AMQManager aMQManager = AMQManager.GetInstance();
        MainWindow1 phoneWindow = null;
        public LoginWindow()
        {

            InitializeComponent();
            phoneWindow = new MainWindow1();
            aMQManager = AMQManager.GetInstance();
            aMQManager.InitializeAMQ();
            aMQManager.SendMessageToQueue(GC_AllCommand.Hello.ToString() + "#192.168.1.148", null);
            aMQManager.messageArrived += AMQManager_messageArrived;
            aMQManager.UpdateTopic();
            Destination = ConfigurationManager.AppSettings["Destination"].ToString();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AMQManager aMQManager = AMQManager.GetInstance();
            agent = Agent.GetInstance();
            agent.AgentID = txtAgentID.Text;
            agent.AgentPassword = txtAgentPassword.Password;
            agent.AgentExtension = txtAgentExtension.Text;




            string command = GC_Utility.CreateComand(GC_AllCommand.Connect.ToString(), agent.AgentID);
            aMQManager.SendMessageToQueue(command, "192.168.1.148," + agent.AgentPassword);
            aMQManager.UpdateTopic();
            bool result = aMQManager.SendMessageToQueue("Login#565656", "Expertflow464,42033");
            //aMQManager.messageArrived += AMQManager_messageArrived;
            aMQManager.UpdateTopic();

            ResetControls();
        }

        private void AMQManager_messageArrived(object sender, MyEventArgs args)
        {
            try
            {
                if (args != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (args.eventArgs[1] == "State")
                        {

                            phoneWindow.txtAgentName.Text = args.eventArgs[7];
                            phoneWindow.txtAgentExtension.Text = "(EXT. " + agent.AgentExtension + ")";
                            agent.AgentCurrentState = args.eventArgs[2];
                            agent.AgentCurrentStateCode = args.eventArgs[3];
                            string reasonCode = GC_Utility.CreateComand(GC_AllCommand.reasoncodenotready.ToString(), agent.AgentID);
                            aMQManager.messageArrived += AMQManager_messageArrived;
                            aMQManager.SendMessageToQueue(reasonCode, null);
                            
                            this.Close();

                            if (!isopen)
                            {
                                aMQManager.messageArrived -= AMQManager_messageArrived;
                                phoneWindow.Show();
                                isopen = true;
                            }
                        }
                        else if (args.eventArgs[1] == GC_Events.ReasonCodes.ToString())
                        {
                            AMQManager aMQManager = AMQManager.GetInstance();
                            string[] codes = args.eventArgs[3].Split(',');
                            string[] labels = args.eventArgs[4].Split(',');

                            List<string> list = labels.ToList();
                            list.Add("Not Ready");
                            list.Add("Ready");
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
                            phoneWindow.cbReasonCodes.Items.Clear();
                            int ind = 0;
                            foreach (var item in labels)
                            {
                                var cbitem = new ComboBoxItem();    

                                if (ind == 0)
                                {
                                    var code = item.Split(':');
                                    cbitem.Content = code[1];

                                }
                                else
                                {
                                    cbitem.Content = item;
                                }
                                phoneWindow.cbReasonCodes.Items.Add(cbitem);
                                ind++;
                            }

                            foreach(ComboBoxItem itm in phoneWindow.cbReasonCodes.Items)
                            {
                                if(itm.Content.ToString() =="Not Ready")
                                {
                                    phoneWindow.cbReasonCodes.SelectedItem = itm;
                                }
                            }
                            aMQManager.messageArrived -= AMQManager_messageArrived;
                        }
                        else if (args.eventArgs[1] == "OUT_OF_SERVICE")
                        {
                            HelloCommandStatus.Text = "System Out of Service";
                            HelloCommandStatus.Visibility = Visibility.Visible;
                        }
                        else if (args.eventArgs[1] == "IN_SERVICE")
                        {
                            HelloCommandStatus.Text = "System Available";
                            HelloCommandStatus.Visibility = Visibility.Hidden;
                        }
                        else if (args.eventArgs[1] == "Error")
                        {
                            HelloCommandStatus.Text = args.eventArgs[3];
                            HelloCommandStatus.Visibility = Visibility.Visible;
                        }

                    });
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        private void ResetControls()
        {
            foreach (TextBox item in sp_TextBoxes.Children.OfType<TextBox>())
            {
                item.Clear();
            }
        }
    }
}
