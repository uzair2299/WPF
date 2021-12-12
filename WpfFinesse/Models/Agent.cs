using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFinesse.Models
{
    public class Agent
    {
        public string AgentID { get; set; }
        public string AgentPassword { get; set; }
        public string AgentExtension { get; set; }

        private static Agent instance = null;
        public string AgentCurrentState { get; set; }
        public string AgentCurrentStateCode { get; set; }

        private Agent()
        {
        }

        public static Agent GetInstance()
        {
            try
            {
                if (instance == null)
                {
                    instance = new Agent();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return instance;
        }
    }
}
