using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskFramework.Lib;

namespace Demo
{
    public partial class frmDemo : Form
    {

        ParallelTaskQueue<SimpleParallelTask> dc = null;

        public frmDemo()
        {
            InitializeComponent();
        }

        private void frmDemo_Load(object sender, EventArgs e)
        {
            listViewEx1.FullRowSelect = true;
            listViewEx1.ProgressColumIndex = 1;

            int parallaCount = Convert.ToInt32(nParallaCount.Value);
            dc = ParallelTaskQueue<SimpleParallelTask>.GetGlobalCollection(parallaCount);

            dc.AllTasksAdded += dc_AllTasksAdded;
            dc.WorkTasksAdded += dc_WorkTasksAdded;
            dc.DoneTasksAdded += dc_DoneTasksAdded;
            dc.OnPause += dc_OnPause;
            dc.OnStop += dc_OnStop;
            dc.OnResume += dc_OnResume;
            dc.OnStoping += dc_OnStoping;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int taskCount = Convert.ToInt32(nTaskCount.Value);
            int parallaCount = Convert.ToInt32(nParallaCount.Value);

            dc.SetParallaCount(parallaCount);

            for (int i = 0; i < taskCount; i++)
            {
                SimpleParallelTask dt = new SimpleParallelTask(computeProgress, i);
                dc.Push(dt);
            }
        }

        void computeProgress(object param)
        {
            var args = param as TaskArgs;

            int id = (int)args.Task.Id;
            Random rm = new Random();
            int delay = rm.Next(200);
            ListViewItem item = null;
            args.Context.Invoke(() =>
            {
                item = listViewEx1.Items.Add(id.ToString(), "", "");
                item.SubItems.Add("0");
                item.SubItems.Add("0");
                item.SubItems[0].Text = id.ToString();
                item.SubItems[2].Text = delay.ToString();
            });
            for (int i = 1; i < 100; i++)
            {
                if (args.Task.CancellationToken.IsCancellationRequested)
                    break;

                System.Threading.Thread.Sleep(delay);
                args.Context.Invoke(() =>
                {
                    item.SubItems[1].Text = i.ToString();
                });
            }
        }


        void dc_OnStoping(object sender, EventArgs e)
        {
            OperEvent();
        }

        void dc_OnResume(object sender, EventArgs e)
        {
            OperEvent();
        }

        void dc_OnStop(object sender, EventArgs e)
        {
            OperEvent();
        }

        void dc_OnPause(object sender, EventArgs e)
        {
            OperEvent();
        }

        void OperEvent()
        {
            dc.GetContext().Invoke(() =>
             {
                 lbDoneTaskCount.Text = dc.DoneTaskCount.ToString();
                 lbWorkTaskCount.Text = dc.WorkTaskCount.ToString();
                 lbAllTaskCount.Text = dc.AllTaskCount.ToString();
             });
        }

        void dc_DoneTasksAdded(object sender, TaskArgs e)
        {
            dc.Invoke(() =>
            {
                lbDoneTaskCount.Text = dc.DoneTaskCount.ToString();
            });

        }

        void dc_WorkTasksAdded(object sender, TaskArgs e)
        {
            //var context = dc.GetContext();

            dc.Invoke(() =>
            {
                textBox1.AppendText("Task: " + e.Task.Id.ToString() + " added.");
                textBox1.AppendText(Environment.NewLine);
                lbWorkTaskCount.Text = dc.WorkTaskCount.ToString();
            });
        }

        void dc_AllTasksAdded(object sender, TaskArgs e)
        {
            //var context = dc.GetContext();

            dc.Invoke(() =>
            {
                lbAllTaskCount.Text = dc.AllTaskCount.ToString();
                lbTaskMax.Text = dc.TaskMax.ToString();
            });
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            dc.Pause();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            dc.Resume();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            dc.Stop();
            listViewEx1.Items.Clear();
        }

        private void nParallaCount_ValueChanged(object sender, EventArgs e)
        {
            dc.SetParallaCount(Convert.ToInt32(nParallaCount.Value));
        }
    }
}
