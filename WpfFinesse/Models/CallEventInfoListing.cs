using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFinesse.AMQ;

namespace WpfFinesse.Models
{
    public  class CallInfoData
    {
        //public CallInfoData();

        public SortedDictionary<string, string> AdditionalParameters { get; set; }
        public string Ani { get; set; }
  //      public CtiCallActionOptions CallControlOptions { get; set; }
        public string CallerName { get; set; }
        public string CallId { get; set; }
        public DateTime CallReceived { get; set; }
        public string CallType { get; set; }
        public string CtiCallState { get; }
        public string CurrentCallState { get; set; }
        public string Dnis { get; set; }
        public Guid GetCtiCallRefId { get; }
        public string IPAddress { get; set; }
        public object NewCallEventObject { get; set; }

//        public void SetCtiCallStateData(string ctiReferenceCallState);
    }

    public class CallEventInfo
    {
        //  private static CallEventInfo instance = null;
        public CallEventInfo()
        {
            this.istimeDifferenceCalculated = false;
        }
        public string AgentID { get; set; }
        public CallInfoData MyCallInfoData { get; set; }
        public List<string> callVariables { get; set; }
        public int Duration { get; set; }
        public TimeSpan timeDifference { get; set; }
        public string callStartTime { get; set; }
        public Stopwatch callstopwatch { get; set; }
        public Stopwatch holdcallstopwatch { get; set; }
        public Stopwatch Wrapupcallstopwatch { get; set; }
        public bool istimeDifferenceCalculated { get; set; }
        public Dictionary<string, string> callvariablesNameValue { get; set; }

        //public  int participantCount { get; set; }

        // public List<string> participants { get; set; }

    }
    public static class CallEventInfoListing
    {
        public static List<CallEventInfo> lstCallEventInfo = new List<CallEventInfo>();
        public static string Teams;

        public static int totalNumberOfTeams;
        public static bool isSupervisor;

        public static string currentActiveCall;

        public static string agentID;



        public static string agentPassword;

        public static string GC;

        public static string agentExtension;

        public static Guid agentGUID;

        public static string previousDialogID;

        public static string activityDialogID;

        public static string agentSate;

        public static string comments;

        public static string SendRequestToCRM { get; set; }

        public static string agentFullName;

        public static object objCTIConnector;

        public static object objBaseCTIDesktopMgr;
        internal static string serverType = "UCCE";
        //private static ILog log = LogManager.GetLogger("CallEventInfoListing");

        public static string errorMessage { get; set; }

        public static string infoMessage { get; set; }

        public static Dictionary<string, string> notReadyItems { get; set; }

        public static Dictionary<string, string> wrapUpReasonLabels { get; set; }

        public static Dictionary<string, string> logoutReasonLabels { get; set; }

        public static bool isWraupAllowed { get; set; }

        public static string notReadyReasonLabel { get; set; }


        public static bool isLoggedIn { get; set; }
        public static string transferCallDialogID { get; set; }
        public static bool isConference { get; set; }
        public static string topicName { get; set; }

        public static string callAction { get; set; }

        public static string consultingAgentExtension { get; set; }

        public static bool isTransferCall { get; set; }

        public static string previousActivityDialogID { get; set; }

        public static string wrapupValue { get; set; }

        public static string dropCallDialogID { get; set; }

        public static string consultPopUpMessage { get; set; }
        public static string globalManager { get; set; }
        public static bool isOutBoundCall { get; set; }

        public static bool isManualOutBoundCall { get; set; }

        public static bool isCallAbandoned { get; set; }

        public static bool isNotReadyItemsMenuPopulated { get; set; }

        public static bool isWrapupitemsPopulated { get; set; }

        public static bool isLogoutReasonsitemsPopulated { get; set; }

        //---------------------New Implementation----------------------

        public static string VTBLGUID { get; set; }
        public static bool isCallLandingPageEventFired { get; set; }

        public static bool isDispositionCallBackFired { get; set; }

        public static bool isLoginEventFired { get; set; }

        public static bool isCallBack { get; set; }
        public static bool isCallBackCallTriggered { get; set; }
        public static string callBackNumber { get; set; }
        public static string callbackVTBLGUID { get; set; }

        public static bool isCustomerFromConferenceDisconnected { get; set; }

        public static string callActionRequested { get; set; }

        public static string ConferenceOwner { get; set; }

        public static int directTransferFromtwoCalls { get; set; }

        //------------------------------------------------------------
        //public static Dictionary<string,string> AuthPin { get; set; }
        public static string AuthPin { get; set; }

        public static List<string> participants { get; set; }




        public static string getParticipantsExtenions()
        {
            string result = string.Empty;
            List<string> lst = new List<string>();
            List<string> lstExt = new List<string>();
            List<string> participantsCopy = CallEventInfoListing.participants;
            foreach (var item in participantsCopy)
            {
                lst = item.Split(',').ToList();
                if (lst[0] != CallEventInfoListing.agentExtension && (lst[1].Equals("ACTIVE") || lst[1].Equals("HELD")))
                {
                    lstExt.Add(lst[0]);
                }
                lstExt.TrimExcess();
            }
            if (lstExt.Count > 0)
            {
                result = string.Join(",", lstExt);
            }

            return result;
        }

        //public static string getStringBetween(string STR)
        //{

        //    string phoneFormatExpression = Utility.GetConfigurationValue("phoneFormatExpression");
        //    string prefixToAddInPhone = Utility.GetConfigurationValue("prefixToAddInPhone");
        //    string prefixToRemoveFromPhone = Utility.GetConfigurationValue("prefixToRemoveFromPhone");

        //    log.Debug("Going to format number.CRM Phone: " + STR + " phoneFormatExpression: " + phoneFormatExpression + " prefixToAddInPhone:" + prefixToAddInPhone + " prefixToRemoveFromPhone:" + prefixToRemoveFromPhone);

        //    string result = Regex.Replace(STR, @phoneFormatExpression, "");

        //    if (!string.IsNullOrEmpty(prefixToRemoveFromPhone) && result.StartsWith(prefixToRemoveFromPhone))
        //    {
        //        result = result.Remove(0, prefixToRemoveFromPhone.Length);
        //    }

        //    if (!string.IsNullOrEmpty(prefixToAddInPhone))
        //    {
        //        result = string.Format("{0}{1}", prefixToAddInPhone, result);
        //    }
        //    //STR = STR.Remove(0, 6);
        //    //var endIdx = STR.Length - 5;
        //    //STR =STR.Remove(endIdx, 5);

        //    //if(!string.IsNullOrEmpty(code) && code.Length > 0)
        //    //     STR = STR.Remove(0, code.Length);
        //    log.Debug("Formatted Number : " + result);
        //    return result.Trim();
        //}

        public static TimeSpan timeDifference(string callStartTime)
        {

            if (!string.IsNullOrEmpty(callStartTime))
            {
                DateTime callTime;
                DateTime.TryParse(callStartTime, out callTime);
                DateTime currentTime = DateTime.Now;
                TimeSpan timeDifference = currentTime.Subtract(callTime);
                return timeDifference;
            }
            return new TimeSpan();
        }

        public static string getValueOfGivenVariable(List<string> tempCallVariables, string variableName)
        {
            string value = string.Empty;

            for (int i = 0; i < tempCallVariables.Count; i++)
            {
                string item = tempCallVariables[i];

                if (!string.IsNullOrEmpty(item))
                {
                    string[] KeyPairValues = item.Split('=');
                    if (KeyPairValues.Length > 1 && !string.IsNullOrEmpty(KeyPairValues[0]) && !string.IsNullOrEmpty(KeyPairValues[1]))
                    {
                        if (KeyPairValues[0].Equals(variableName, StringComparison.OrdinalIgnoreCase))
                        {
                            value = KeyPairValues[1];
                            break;
                        }
                    }

                }
            }

            return value;
        }

        public static bool isGivenVariableExists(List<string> tempCallVariables, string variableName)
        {
            bool value = false;
            for (int i = 0; i < tempCallVariables.Count; i++)
            {
                string item = tempCallVariables[i];
                if (!string.IsNullOrEmpty(item))
                {
                    string[] KeyPairValues = item.Split('=');
                    if (KeyPairValues.Length > 1 && !string.IsNullOrEmpty(KeyPairValues[0]) && !string.IsNullOrEmpty(KeyPairValues[1]))
                    {
                        if (KeyPairValues[0].Equals(variableName)) //, StringComparison.OrdinalIgnoreCase
                        {
                            value = true;
                            break;
                        }
                    }
                }
            }
            return value;
        }
    }


    public class CtiCommands
    {
       // private static ILog log = LogManager.GetLogger("CtiCommands");
        public static void GetCurrentDialogState()
        {
            if (!string.IsNullOrEmpty(CallEventInfoListing.currentActiveCall))
            {
                AMQManager.GetInstance().SendMessageToQueue("GetDialogState#" + CallEventInfoListing.agentID, CallEventInfoListing.currentActiveCall);
                //log.Debug("sent message: GetDialogState#" + CallEventInfoListing.agentID + "," + CallEventInfoListing.currentActiveCall);
            }
            else
            {
            }
        }
        public static void GetAgentState()
        {
            AMQManager.GetInstance().SendMessageToQueue("GetState#" + CallEventInfoListing.agentID, "");

            //log.Debug("CtiCommands-Command send to GC : GetState#" + CallEventInfoListing.agentID);
        }

        public static void GetDialogState(string callId)
        {
            if (!string.IsNullOrEmpty(callId))
            {
                AMQManager.GetInstance().SendMessageToQueue("GetDialogState#" + CallEventInfoListing.agentID, callId);
                //log.Debug("CtiCommands-Command send to GC : GetDialogState#" + CallEventInfoListing.agentID + "," + callId);
            }
            else
            {
            }
        }
        public static void ForceLogout()
        {
            AMQManager.GetInstance().SendMessageToQueue("force_logout#" + CallEventInfoListing.agentID, "");
            //log.Debug("CtiCommands-Command send to GC : force_logout#" + CallEventInfoListing.agentID);
        }
    }
}
