using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFinesse.GC_Commands
{
    enum GC_AllCommand
    {
        Hello,
        Connect,
        AnswerCall,
        ReleaseCall,
        reasoncodenotready,
        MakeReady,
        Transfer_sst,
        HoldCall,
        RetrieveCall,
        ConsultCall
    }

    enum Inboundcall_current_state
    {
         INITIATING
        ,INITIATED
        ,ALERTING
        ,ACTIVE
        ,FAILED
        ,DROPPED
        ,ACCEPTED
            ,HELD
    }


}
