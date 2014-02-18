using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace TaskFramework.Lib.DistributedComputingService
{
    public class DistributedComputingServiceClient
    { 
        public static void CloseChannel<ISvc>(ChannelFactory<ISvc> Factory)
        {
            try
            {
                Factory.Close();
            }
            catch
            {
            }
        }
        public static ChannelFactory<ISvc> CreateChannelFactory<ISvc>(string RemoteAddress, string ServiceName, bool isDuplex = false,IDistributedComputingCallback callback = null)
        {
            string theRemoteAddress = RemoteAddress;
            if (theRemoteAddress.ToLower().IndexOf("net.tcp://") != 0)
            {
                theRemoteAddress = "net.tcp://" + RemoteAddress;
            }
            if (theRemoteAddress[theRemoteAddress.Length - 1] != '/')
            {
                theRemoteAddress += "/";
            }
            NetTcpBinding theBinding = new NetTcpBinding();
            theBinding.MaxReceivedMessageSize = int.MaxValue;
            theBinding.MaxBufferSize = int.MaxValue;
            theBinding.MaxBufferPoolSize = int.MaxValue;
            theBinding.ReaderQuotas.MaxDepth = 32;
            theBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            theBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            theBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            theBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            
            theBinding.Security.Mode = SecurityMode.None;
            theBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            theBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
            theBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

            theBinding.ReceiveTimeout = new TimeSpan(0, 20, 0);
            theBinding.OpenTimeout = new TimeSpan(0, 10, 0);
            theBinding.CloseTimeout = new TimeSpan(0, 10, 0);

            ServiceEndpoint httpEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(ISvc)),
                theBinding,  new EndpointAddress(theRemoteAddress + ServiceName));

            foreach (var op in httpEndpoint.Contract.Operations)
            {
                var dataContractBehavior = op.Behaviors[typeof(DataContractSerializerOperationBehavior)] as DataContractSerializerOperationBehavior;
                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }

            if(!isDuplex)
                return new ChannelFactory<ISvc>(httpEndpoint);
            else
            {
                 InstanceContext instanceContext = new InstanceContext(callback);
                 DuplexChannelFactory<ISvc> channelFactory = new DuplexChannelFactory<ISvc>(instanceContext, httpEndpoint);
                 return channelFactory;
            }
        }



    }
    
}
