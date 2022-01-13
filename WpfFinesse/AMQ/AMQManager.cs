using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFinesse.AMQ
{
    public class AMQManager
    {
        public delegate void MessageArrivedHandler(object sender, MyEventArgs args);

        public event MessageArrivedHandler messageArrived;

        IConnection connection;
        private ISession producerSession;
        private ISession consumerSession;
        private IMessageProducer producer;
        private IMessageConsumer consumer;
        private ITopic topic;
        private IQueue queue;
        private static AMQManager instance = null;
        public string CurrentState = null;
        public string dialog_id = null;
        public Dictionary<string, string> dictionary = new Dictionary<string, string>();


        private AMQManager()
        {
        }

        public static AMQManager GetInstance()
        {
            try
            {
                if (instance == null)
                {
                    instance = new AMQManager();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return instance;
        }

        public void InitializeAMQ()
        {
            try
            {
                IConnectionFactory factory = new ConnectionFactory("tcp://192.168.1.148:61616");
                connection = factory.CreateConnection();
                connection.Start();
                producerSession = connection.CreateSession();
                consumerSession = connection.CreateSession();
                queue = producerSession.GetQueue("Connector1");
                topic = producerSession.GetTopic("192.168.1.148");
                producer = producerSession.CreateProducer(queue);
                consumer = consumerSession.CreateConsumer(topic);
                //consumer.Listener += Consumer_Listener;
                consumer.Listener += new MessageListener(Consumer_Listener);
                producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SendMessageToQueue(string NMSType, string messageText)
        {

            bool sent = false;
            try
            {
                ITextMessage request = this.producerSession.CreateTextMessage(messageText);
                request.NMSType = NMSType;
                request.NMSPriority = MsgPriority.Lowest;
                //request.NMSReplyTo = topic;
                //request.NMSCorrelationID = Guid.NewGuid().ToString();
                producer.Send(request);
                sent = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return sent;
        }

        public  void Consumer_Listener(IMessage message)
        {
            ITextMessage textMessage = message as ITextMessage;
            string[] messageData = textMessage.Text.Split('#');
            MyEventArgs args = new MyEventArgs();
            args.eventArgs = messageData;
            var correlationID = Guid.NewGuid().ToString();
            OnMessageArrived(this, args);
            //     if (messageData[1] == GC_Events.IN_SERVICE.ToString())

            //{
            //    Console.WriteLine("\n\rReceived Event: " + messageData[1] + ":" + ((ITextMessage)message).Text + "/" + DateTime.Now);
            //}
            //else if (messageData[1] == GC_Events.OUT_OF_SERVICE.ToString())
            //{
            //    Console.WriteLine("\n\rReceived Event: " + messageData[1] + ":" + ((ITextMessage)message).Text + "/" + DateTime.Now);
            //}
            //else if (messageData[1] == GC_Events.AgentInfo.ToString())
            //{
            //    Console.WriteLine("\n\rReceived Event: " + messageData[1] + ":" + DateTime.Now);
            //    Console.WriteLine(String.Format("Agent Login Id : {0} \nAgent first Name : {1} \nAgent Last Name : {2} ", messageData[0], messageData[2], messageData[3]));
            //}
            //else if (messageData[1] == GC_Events.State.ToString())
            //{
            //    Console.Clear();
            //    Console.WriteLine("\n\rReceived Event: " + messageData[1] + ":" + DateTime.Now);
            //    Console.WriteLine(String.Format("Agent Login Id : {0} \nAgent Current State : {1} \nReason Code : {2} \nAgent Full Name : {3} ", messageData[0], messageData[2], messageData[3], messageData[7]));
            //    AMQManager aMQManager = AMQManager.GetInstance();
            //    aMQManager.CurrentState = messageData[2];
            //}
            //else if (messageData[1] == GC_Events.DialogStatus.ToString())
            //{
            //    Console.WriteLine("\n\rReceived Event: " + messageData[1] + ":" + DateTime.Now);
            //    //Console.WriteLine(String.Format("Agent Login Id : {0} \n\r Dialog ID : {1} \n\r Reason Code : {2}", messageData[0], messageData[2]));
            //}
            //else if (messageData[1] == GC_Events.ReasonCodes.ToString())
            //{

            //    AMQManager aMQManager = AMQManager.GetInstance();
            //    string[] codes = messageData[3].Split(',');
            //    string[] labels = messageData[4].Split(',');
            //    if (codes.Length == labels.Length)
            //    {
            //        aMQManager.dictionary.Clear();
            //        for (int i = 0; i < codes.Length; i++)
            //        {
            //            aMQManager.dictionary.Add(codes[i], labels[i]);
            //        }
            //    }
            //    Console.Clear();
            //    foreach (KeyValuePair<string, string> entry in aMQManager.dictionary)
            //    {
            //        Console.WriteLine(entry.Key + "-" + entry.Value);
            //    }

            //    Console.WriteLine("enter reason code");
            //    string userinput = Console.ReadLine();
            //    aMQManager.SendMessageToQueue("MakeNotReadyWithReason#ehtasham4", userinput);

            //}
            //else if (messageData[1] == GC_Events.InboundCall.ToString())
            //{
            //    Console.Clear();
            //    //Console.WriteLine("\n\rReceived Event: " + messageData[1] + ":" + DateTime.Now);
            //    Console.WriteLine(String.Format("Inboundcall_current_state : {0} \nDialog ID : {1} \nfromAddress : {2}", messageData[2], messageData[3], messageData[4]));
            //    //Thread.Sleep(5000);
            //    AMQManager aMQManager = AMQManager.GetInstance();
            //    Console.WriteLine("press 2 to end  the call");
            //    Console.WriteLine("press 3  to Blind Transfer the call");
            //    Console.WriteLine("press 4  to ConsultCall the call");
            //    ConsoleKeyInfo ckey = Console.ReadKey();
            //    AMQManager aMQManager1 = AMQManager.GetInstance();
            //    if (ckey.Key == ConsoleKey.NumPad2)
            //    {
            //        Console.Clear();
            //        aMQManager.SendMessageToQueue("ReleaseCall#ehtasham4", aMQManager.dialog_id);
            //    }
            //    else if (ckey.Key == ConsoleKey.NumPad3)
            //    {
            //        Console.Clear();
            //        aMQManager.SendMessageToQueue("Transfer_sst#ehtasham4", "2202," + aMQManager.dialog_id);
            //    }
            //    else if (ckey.Key == ConsoleKey.NumPad4)
            //    {
            //        Console.Clear();
            //        aMQManager.SendMessageToQueue("ConsultCall#ehtasham4", "2202," + aMQManager.dialog_id);
            //    }

            //}
            //else if (messageData[1] == GC_Events.NewInboundCall.ToString())
            //{
            //    Console.Clear();
            //    string[] dialogID = messageData[4].Split(':');
            //    Console.WriteLine(String.Format("Calling : {0} ", messageData[2]));
            //    Console.WriteLine(String.Format("Dialog ID : {0} ", dialogID[1]));
            //    AMQManager aMQManager = AMQManager.GetInstance();
            //    aMQManager.dialog_id = dialogID[1];
            //    Console.WriteLine("press 1 to response the call");
            //    ConsoleKeyInfo ckey = Console.ReadKey();
            //    if (ckey.Key == ConsoleKey.NumPad1)
            //    {
            //        Console.Clear();
            //        aMQManager.SendMessageToQueue("AnswerCall#ehtasham4", dialogID[1]);
            //    }
            //}

            //else
            //{
            //    Console.WriteLine("\n\rReceived Event: " + DateTime.Now + "::" + message);
            //}


        }

        protected virtual void OnMessageArrived(object sender, MyEventArgs e)
        {
            messageArrived?.Invoke(sender, e);
        }

        public void UpdateTopic()
        {
            try
            {
                consumer.Listener -= Consumer_Listener;
                topic = consumerSession.GetTopic("192.168.1.148");
                consumer = consumerSession.CreateConsumer(topic);
                consumer.Listener += Consumer_Listener;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

    public class MyEventArgs : EventArgs
    {
        public string[] eventArgs { get; set; }

    }
    enum GC_Events
    {
        IN_SERVICE,
        OUT_OF_SERVICE,
        AgentInfo,
        State,
        DialogStatus,
        ReasonCodes,
        NewInboundCall,
        InboundCall,
    }
}
