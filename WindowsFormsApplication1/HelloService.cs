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
    public class HelloService : IDistributedComputingWebService
    {

        public string WebInvokeAction(string actionName, string param)
        {
            return "hello web:" + actionName +param; 
        }
    }
}
