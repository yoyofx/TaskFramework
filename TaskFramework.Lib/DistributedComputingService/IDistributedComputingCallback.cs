using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace TaskFramework.Lib.DistributedComputingService
{
    public interface IDistributedComputingCallback
    {
        [OperationContract]
        void OnSerivceDataCallBack(string actionName, string data);

        [OperationContract]
        void OnSerivceStreamCallBack(string actionName, byte[] data);
    }
}
