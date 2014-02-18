using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using TaskFramework.Lib.DistributedComputingService;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //net.tcp://localhost:12581/{0}/
            ChannelFactory<IDistributedComputingDuplexService> channelFactory = 
                DistributedComputingServiceClient.CreateChannelFactory<IDistributedComputingDuplexService>
                                                                                                        ("net.tcp://localhost:12581/", "Lss", true, new MyDateCallBack());
            
                var channel =  channelFactory.CreateChannel();
                string hello = channel.NetInvokeAction("login","maxzhang");


            
        }

  
    }

    public class MyDateCallBack : IDistributedComputingCallback
    {

        public void OnSerivceDataCallBack(string actionName, string data)
        {
            MessageBox.Show(data, actionName);
        }

        public void OnSerivceStreamCallBack(string actionName, byte[] data)
        {
            throw new NotImplementedException();
        }
    }


}
