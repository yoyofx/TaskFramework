using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace TaskFramework.Lib.DistributedComputingService
{
    public class  DistributedComputingServiceHost
    {
        internal static ServiceHost webHost;
        internal static ServiceHost tcpHost;
        private static Binding CreateTcpBinding()
        {
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
            return theBinding;
        }

        private static Binding CreateWebBinding()
        {
            WebHttpBinding theBinding = new WebHttpBinding();
            theBinding.MaxReceivedMessageSize = int.MaxValue;
            theBinding.MaxBufferSize = int.MaxValue;
            theBinding.MaxBufferPoolSize = int.MaxValue;
            theBinding.ReaderQuotas.MaxDepth = 32;
            theBinding.ReaderQuotas.MaxStringContentLength = 2147483647;
            theBinding.ReaderQuotas.MaxArrayLength = 2147483647;
            theBinding.ReaderQuotas.MaxBytesPerRead = 2147483647;
            theBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
            theBinding.Security.Mode = WebHttpSecurityMode.None;
            theBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            return theBinding;
        }

        public static void StartNetService<TSvc>(string serviceName) where TSvc : IDistributedComputingDuplexService
        {
            var netTcpUri = new Uri(string.Format("net.tcp://localhost:12581/{0}/", serviceName));
            tcpHost = new ServiceHost(typeof(TSvc));
            Binding theTcpBinding = CreateTcpBinding();
            //tcp host
            tcpHost.AddServiceEndpoint(typeof(IDistributedComputingDuplexService), theTcpBinding, netTcpUri);
            tcpHost.Open();
        }

        public static void StartWebService<TSvc>(string serviceName) where TSvc : IDistributedComputingWebService 
        {
            //var netTcpUri = new Uri(string.Format("net.tcp://localhost:12581/{0}/", serviceName));
            //tcpHost = new ServiceHost(typeof(TSvc));
            //Binding theTcpBinding = CreateTcpBinding();
            ////tcp host
            //tcpHost.AddServiceEndpoint(typeof(IDistributedComputingService), theTcpBinding, netTcpUri);
            //web host
            var webHttpUri = new Uri(string.Format("http://localhost:12580/{0}/", serviceName));
            webHost = new ServiceHost(typeof(TSvc));
            Binding theWebBinding = CreateWebBinding();
            webHost.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true, HttpGetUrl = webHttpUri });
            var endPoint = webHost.AddServiceEndpoint(typeof(IDistributedComputingWebService), theWebBinding, webHttpUri);
            endPoint.Behaviors.Add(new WebHttpBehavior() { HelpEnabled = true });

            //tcpHost.Open();
            webHost.Open();
        }
        public static void StopService()
        {
            try
            {
                tcpHost.Close();
                webHost.Close();
            }
            catch
            {
            }
        }
    }
}
