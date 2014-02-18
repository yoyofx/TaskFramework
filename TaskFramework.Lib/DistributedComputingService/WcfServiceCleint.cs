using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace TaskFramework.Lib.DistributedComputingService
{
    public class WcfServiceCleint
    {
            public DateTime NowdateTime { get; set; }
            public IDistributedComputingCallback callbackHandler { get; set; }

            public event EventHandler ClientClosed;


            public WcfServiceCleint(IDistributedComputingCallback callback, DateTime nowTime)
            {
                OperationContext.Current.Channel.Closed += Channel_Closed;
                this.callbackHandler = callback;
                this.NowdateTime = nowTime;
            }

           public void Notification(string actionName,string msg)
           {
               try
               {
                   callbackHandler.OnSerivceDataCallBack(actionName,msg);
               }
               catch { }
           }

           public void Notification(string actionName, byte[] msg)
           {
               try
               {
                   callbackHandler.OnSerivceStreamCallBack(actionName, msg);
               }
               catch { }
           }


            void Channel_Closed(object sender, EventArgs e)
            {
                if (ClientClosed != null)
                    ClientClosed.Invoke(this,null);
            }
        }

    
}
