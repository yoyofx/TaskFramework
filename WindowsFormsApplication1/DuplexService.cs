using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TaskFramework.Lib.DistributedComputingService;

namespace WindowsFormsApplication1
{

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class DuplexService : IDistributedComputingDuplexService
    {
        public void Connect(string uid)
        {
            throw new NotImplementedException();
        }

        public void DisConnect(string uid)
        {
            throw new NotImplementedException();
        }

        public void Online(string uid)
        {
            throw new NotImplementedException();
        }

        public string NetInvokeAction(string actionName, string param)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IDistributedComputingCallback>();
            Task.Factory.StartNew(new Action<object>((s) =>
            {
                System.Threading.Thread.Sleep(2500);
                var cb = s as IDistributedComputingCallback;

                cb.OnSerivceDataCallBack("tcpTest", "hello world" + DateTime.Now.ToString());
            }), callback);


            return "hello tcp:" + param;
        }

        public string NetInvokeActionStream(string actionName, byte[] param)
        {
            throw new NotImplementedException();
        }
    }

  
}
