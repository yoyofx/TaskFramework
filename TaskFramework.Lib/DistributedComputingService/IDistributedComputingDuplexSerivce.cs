using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace TaskFramework.Lib.DistributedComputingService
{
     [ServiceContract(CallbackContract = typeof(IDistributedComputingCallback))]
    public interface IDistributedComputingDuplexService
    {
         [OperationContract]
         void Connect(string uid);

         [OperationContract]
         void DisConnect(string uid);

          [OperationContract]
         void Online(string uid);

          [OperationContract]
          string NetInvokeAction(string actionName, string param);
          [OperationContract]
          string NetInvokeActionStream(string actionName, byte[] param);

    }
}
