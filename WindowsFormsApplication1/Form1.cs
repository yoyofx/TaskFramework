using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaskFramework.Lib.DistributedComputingService;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //http://localhost:8085/Lss
            DistributedComputingServiceHost.StartWebService<HelloService>("Lss");
            
            
            DistributedComputingServiceHost.StartNetService<DuplexService>("Lss");
           
        }
    }
}
