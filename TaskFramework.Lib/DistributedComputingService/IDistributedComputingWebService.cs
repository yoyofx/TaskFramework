using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.IO;
namespace TaskFramework.Lib.DistributedComputingService
{

    [ServiceContract]
    public interface IDistributedComputingWebService
    {
        /// <summary>
        /// 基于Web REST的服务执行
        /// </summary>
        /// <param name="actionName">动作名称</param>
        /// <param name="param">参数（可能是JSON）</param>
        /// <returns>结果（可能是JSON）</returns>
        [WebGet(UriTemplate = "/Invoke/{actionName}/{param}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        string WebInvokeAction(string actionName,string param);


    }
}
